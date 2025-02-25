package director

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"math/rand"
	"net/http"
	"os"
	"strconv"
	"sync"
	"time"
	models "warcos-director/internal/models"

	agonesv1 "agones.dev/agones/pkg/apis/agones/v1"
	"agones.dev/agones/pkg/client/clientset/versioned"
	"github.com/pkg/errors"
	"google.golang.org/grpc"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/structpb"
	corev1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
	"k8s.io/apimachinery/pkg/util/wait"
	"k8s.io/client-go/rest"
	"open-match.dev/open-match/pkg/pb"
)

// The Director continously polls Open Match for the Match Profiles
// and makes assignments for the Tickets in the returned matches.

const (
	omBackendEndpoint            = "open-match-backend.open-match.svc.cluster.local:50505"
	lobbyUrlEnv                  = "LOBBY_URL"
	profileEnv                   = "PROFILE"
	gameServerEnv                = "GAME_SERVER_IMAGE"
	startGameEndpoint            = "/api/private/startGame"
	cancelGameSearchEndpoint     = "/api/private/cancelGameSearchByTicketIds"
	getCurrentGameRegimeEndpoint = "/api/private/getCurrentGameRegime?gameMode="
	getGameModesEndpoint         = "/api/private/getGameModes"
	userToTeamLabel              = "user_to_team"
	userToSquadLabel             = "user_to_squad"
	fleetNameLabel               = "warcos-game-server"
	imagePullSecretName          = "warcos-game-registry"
	jsonContentType              = "application/json"
	mmfHostName                  = "warcos-matchfunction.warcos-mmf.svc.cluster.local"
	mmfPort                      = 50502
)

var (
	agonesClient    *versioned.Clientset
	gameServerImage string
	isDevelopment   = false
)

func Start() {
	// Check environment variables
	_, present := os.LookupEnv(lobbyUrlEnv)
	if !present {
		panic(fmt.Sprintf("Environment variable %s is not set!", lobbyUrlEnv))
	}
	profile, present := os.LookupEnv(profileEnv)
	if present && profile == "dev" {
		isDevelopment = true
	}
	gameServerImage, present = os.LookupEnv(gameServerEnv)
	if !present {
		panic(fmt.Sprintf("Environment variable %s is not set!", gameServerEnv))
	}

	// Connect to Open Match Backend.
	conn, err := grpc.Dial(omBackendEndpoint, grpc.WithInsecure())
	if err != nil {
		panic(fmt.Sprintf("Failed to connect to Open Match Backend, got %s", err.Error()))
	}

	defer conn.Close()
	be := pb.NewBackendServiceClient(conn)
	agonesClient = getAgonesClient()

	// Generate the profiles to fetch matches for.
	gameModes, err := getAllGameModes()
	if err != nil {
		panic(err)
	}
	profiles, err := GenerateProfiles(gameModes)
	if err != nil {
		panic(err)
	}

	log.Printf("Fetching matches for %v profiles", len(profiles))
	for range time.Tick(time.Second * 5) {
		// Fetch matches for each profile and make random assignments for Tickets in
		// the matches returned.
		var wg sync.WaitGroup
		for _, p := range profiles {
			wg.Add(1)
			go func(wg *sync.WaitGroup, p *pb.MatchProfile) {
				defer wg.Done()
				matches, err := fetch(be, p)
				if err != nil {
					log.Printf("Failed to fetch matches for profile %v, got %s", p.GetName(), err.Error())
					return
				}

				log.Printf("Generated %v matches for profile %v", len(matches), p.GetName())
				assignMatches(be, matches, p)
			}(&wg, p)
		}

		wg.Wait()
	}
}

// initialize kubernetes-API client
func getAgonesClient() *versioned.Clientset {
	if isDevelopment {
		return nil
	}
	config, err := rest.InClusterConfig()
	if err != nil {
		log.Printf("Could not create in cluster config: %s", err)
		panic(err)
	}

	// Access to the Agones resources through the Agones Clientset
	// Note that we reuse the same config as we used for the Kubernetes Clientset
	agonesClient, err := versioned.NewForConfig(config)
	if err != nil {
		log.Printf("Could not create agones client: %s", err)
		panic(err)
	}
	log.Printf("Created the agones api clientset")
	return agonesClient
}

func fetch(be pb.BackendServiceClient, p *pb.MatchProfile) ([]*pb.Match, error) {
	req := &pb.FetchMatchesRequest{
		Config: &pb.FunctionConfig{
			Host: mmfHostName,
			Port: mmfPort,
			Type: pb.FunctionConfig_GRPC,
		},
		Profile: p,
	}

	stream, err := be.FetchMatches(context.Background(), req)
	if err != nil {
		log.Println(err)
		return nil, err
	}

	var result []*pb.Match
	for {
		resp, err := stream.Recv()
		if err == io.EOF {
			break
		}

		if err != nil {
			return nil, err
		}

		result = append(result, resp.GetMatch())
	}

	log.Printf("Fetched matches: %d", len(result))
	return result, nil
}

func assignMatches(be pb.BackendServiceClient, matches []*pb.Match, profile *pb.MatchProfile) {
	for _, match := range matches {
		err := Assign(be, match, profile)
		if err != nil {
			log.Printf("Failed to assign servers to match, got %s", err.Error())
		}
	}
}

func Assign(be pb.BackendServiceClient, match *pb.Match, profile *pb.MatchProfile) error {
	ticketIDs := []string{}
	for _, t := range match.GetTickets() {
		ticketIDs = append(ticketIDs, t.Id)
	}
	gameMode := profile.Pools[0].TagPresentFilters[0].Tag
	currentGameRegime, err := getCurrentGameRegime(gameMode)
	if err != nil {
		return fmt.Errorf("failed to determine game regime for game mode %s at %s, got error %w",
			gameMode, time.Now().Format("2006-01-02T15:04:05.00"), err)
	}

	// Parse match extensions
	userToTeam, err := ParseMapFromMatchExtensions(match, userToTeamLabel)
	if err != nil {
		return err
	}
	userToSquad, err := ParseMapFromMatchExtensions(match, userToSquadLabel)
	if err != nil {
		return err
	}

	gameServerUrl, err := createNewServer(currentGameRegime, userToTeam, userToSquad)
	if err != nil {
		error := callCancelGameSearch(ticketIDs)
		if error != nil {
			log.Printf("Couldn't cancel game search by ticket ids %s, got %s", ticketIDs, error.Error())
		}
		return fmt.Errorf("failed to allocate game server for game settings %v, got error %w", currentGameRegime, err)
	}
	err = callAssignTickets(be, ticketIDs, gameServerUrl)
	if err != nil {
		return fmt.Errorf("AssignTickets failed for match %v, got %w", match.GetMatchId(), err)
	}
	err = callStartGame(currentGameRegime.GameRegimeEntryId, gameServerUrl, userToTeam, userToSquad, ticketIDs)
	if err != nil {
		return fmt.Errorf("calling startGame failed, got %w", err)
	}

	log.Printf("Assigned server %v to match %v", gameServerUrl, match.GetMatchId())
	return nil
}

var callAssignTickets = func(be pb.BackendServiceClient, ticketIDs []string, gameServerUrl string) error {
	req := &pb.AssignTicketsRequest{
		Assignments: []*pb.AssignmentGroup{
			{
				TicketIds: ticketIDs,
				Assignment: &pb.Assignment{
					Connection: gameServerUrl,
				},
			},
		},
	}

	_, err := be.AssignTickets(context.Background(), req)
	return err
}

func ParseMapFromMatchExtensions(match *pb.Match, fieldName string) (map[string]int, error) {
	extensionField, ok := match.Extensions[fieldName]
	if !ok {
		return nil, fmt.Errorf("cannot find %s in match extensions", fieldName)
	}
	var mapRaw structpb.Value
	err := anypb.UnmarshalTo(extensionField, &mapRaw, proto.UnmarshalOptions{})
	if err != nil {
		return nil, err
	}
	extension := mapRaw.GetStructValue()
	if extension == nil {
		return nil, fmt.Errorf("%s in match is not a Struct", fieldName)
	}
	result := map[string]int{}
	for key, value := range extension.Fields {
		result[key] = int(value.GetNumberValue())
	}
	return result, nil
}

func createNewServer(gameRegime *models.ScheduledGameRegime, userToTeam map[string]int, userToSquad map[string]int) (string, error) {
	log.Printf("Creating a new game server with game regime: %v", gameRegime.GameMode)
	if isDevelopment {
		return generateRandomUrl(), nil
	}
	// Create a GameServerAllocation
	gameSettings := models.GameSettings{
		UserToTeam:  userToTeam,
		UserToSquad: userToSquad,
		Map:         gameRegime.MapKind,
		GameMode:    gameRegime.GameMode,
		GameRegime:  gameRegime.GameRegime,
		IsTeam:      gameRegime.IsTeam,
	}
	gameSettingsJson, err := json.Marshal(gameSettings)
	if err != nil {
		log.Printf("Couldn't create game settings json, got: %s\n", err)
		return "", err
	}

	lists := map[string]agonesv1.ListStatus{}
	lists["gameSettings"] = agonesv1.ListStatus{
		Values: []string{string(gameSettingsJson)},
	}
	gameServerName := fmt.Sprintf("warcos-game-server-%+v", randSeq(5))
	gs := &agonesv1.GameServer{
		ObjectMeta: metav1.ObjectMeta{
			Name: gameServerName,
		},
		Spec: agonesv1.GameServerSpec{
			Lists: lists,
			Health: agonesv1.Health{
				// TODO: false
				Disabled: false,
			},
			Ports: []agonesv1.GameServerPort{{
				ContainerPort: 7777,
				Name:          "default",
			}},
			Template: corev1.PodTemplateSpec{
				Spec: corev1.PodSpec{
					Containers: []corev1.Container{{
						Name:            gameServerName,
						Image:           gameServerImage,
						ImagePullPolicy: corev1.PullIfNotPresent,
					}},
					ImagePullSecrets: []corev1.LocalObjectReference{
						{
							Name: imagePullSecretName,
						},
					},
				},
			},
		},
	}
	gameserver, err := agonesClient.AgonesV1().GameServers("default").Create(context.TODO(), gs, metav1.CreateOptions{})
	if err != nil {
		log.Printf("Could not create new GameServer: %s", err)
		return "", err
	}
	log.Printf("GameServer %s created, waiting for Ready", gameServerName)
	gameserver, err = WaitForGameServerState(gameserver, agonesv1.GameServerStateReady, time.Duration(40*time.Second))
	if err != nil {
		log.Printf("Timeout while waiting for Ready status for new GameServer: %s", err)
		return "", err
	}
	gameServerUrl := fmt.Sprintf("%s:%d", gameserver.Status.Address, gameserver.Status.Ports[0].Port)

	log.Printf("The url of a new game server is %s; name is %s", gameServerUrl, gameServerName)
	return gameServerUrl, err
}

const letterBytes = "abcdefghijklmnopqrstuvwxyz"

func randSeq(n int) string {
	b := make([]byte, n)
	for i := range b {
		b[i] = letterBytes[rand.Intn(len(letterBytes))]
	}
	return string(b)
}

// WaitForGameServerState Waits untils the gameserver reach a given state before the timeout expires (with a default logger)
func WaitForGameServerState(gs *agonesv1.GameServer, state agonesv1.GameServerState, timeout time.Duration) (*agonesv1.GameServer, error) {
	var checkGs *agonesv1.GameServer

	err := wait.PollUntilContextTimeout(context.Background(), 1*time.Second, timeout, true, func(_ context.Context) (bool, error) {
		var err error
		checkGs, err = agonesClient.AgonesV1().GameServers(gs.Namespace).Get(context.Background(), gs.Name, metav1.GetOptions{})

		if err != nil {
			log.Printf("Could not retrieve new GameServer info: %s", err)
			return false, nil
		}

		checkState := checkGs.Status.State
		if checkState == state {
			return true, nil
		}
		log.Printf("Waiting for states to match. Game server %s, current state %s, target state %s",
			checkGs.ObjectMeta.Name, checkState, state)

		return false, nil
	})

	return checkGs, errors.Wrapf(err, "waiting for GameServer %v/%v to be %v",
		gs.Namespace, gs.Name, state)
}

func createStartGameRequestData(
	gameRegimeEntryId string, gameServerUrl string, userToTeam map[string]int, userToSquad map[string]int,
	ticketIDs []string,
) ([]byte, error) {
	requestData, err := json.Marshal(models.StartGameRequest{
		GameRegimeEntryId: gameRegimeEntryId,
		GameServerUrl:     gameServerUrl,
		UserToTeam:        userToTeam,
		UserToSquad:       userToSquad,
		TicketIds:         ticketIDs,
	})
	if err != nil {
		return nil, err
	}
	return requestData, nil
}

func createCancelGameSearchData(
	ticketIDs []string,
) ([]byte, error) {
	requestData, err := json.Marshal(ticketIDs)
	if err != nil {
		return nil, err
	}
	return requestData, nil
}

var callStartGame = func(
	gameRegimeEntryId string, gameServerUrl string, userToTeam map[string]int, userToSquad map[string]int, ticketIDs []string,
) error {
	requestBody, err := createStartGameRequestData(gameRegimeEntryId, gameServerUrl, userToTeam, userToSquad, ticketIDs)
	if err != nil {
		return err
	}

	log.Printf("Calling start of the game with payload %s", requestBody)
	lobbyUrl := os.Getenv(lobbyUrlEnv)
	url := lobbyUrl + startGameEndpoint
	resp, err := http.Post(url, jsonContentType, bytes.NewBuffer(requestBody))
	if err != nil {
		return err
	}
	defer resp.Body.Close()
	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("callStartGame: Non-OK HTTP status %d", resp.StatusCode)
	}
	return nil
}

var callCancelGameSearch = func(
	ticketIDs []string,
) error {
	requestBody, err := createCancelGameSearchData(ticketIDs)
	if err != nil {
		return err
	}

	log.Printf("Calling cancel game search with payload %s", requestBody)
	lobbyUrl := os.Getenv(lobbyUrlEnv)
	url := lobbyUrl + cancelGameSearchEndpoint
	resp, err := http.Post(url, jsonContentType, bytes.NewBuffer(requestBody))
	if err != nil {
		return err
	}
	defer resp.Body.Close()
	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("callCancelGameSearch: Non-OK HTTP status %d", resp.StatusCode)
	}
	return nil
}

var getCurrentGameRegime = func(gameMode string) (*models.ScheduledGameRegime, error) {
	lobbyUrl := os.Getenv(lobbyUrlEnv)
	url := lobbyUrl + getCurrentGameRegimeEndpoint + gameMode
	resp, err := http.Get(url)
	if err != nil {
		log.Printf("error when getting current game regime for game mode %s: %s\n", gameMode, err)
		return nil, err
	}
	defer resp.Body.Close()

	var result models.ScheduledGameRegime
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Println("Cannot get current game regime from response")
	}
	if err := json.Unmarshal(body, &result); err != nil {
		log.Println("Cannot unmarshal current game regime JSON")
		return nil, err
	}

	return &result, nil
}

func getAllGameModes() ([]*models.GameMode, error) {
	lobbyUrl := os.Getenv(lobbyUrlEnv)
	url := lobbyUrl + getGameModesEndpoint
	resp, err := http.Get(url)
	if err != nil {
		log.Printf("error when getting game modes: %s\n", err)
		return nil, err
	}
	defer resp.Body.Close()

	var result []*models.GameMode
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		log.Println("Cannot reading game modes from response")
	}
	if err := json.Unmarshal(body, &result); err != nil {
		log.Println("Cannot unmarshal game modes JSON")
		return nil, err
	}

	return result, nil
}

func generateRandomUrl() string {
	url := ""
	for i := 0; i < 4; i++ {
		if i > 0 {
			url += "."
		}
		n := rand.Intn(255) + 1
		url += strconv.Itoa(n)
	}
	port := rand.Intn(9000) + 1000
	url += ":" + strconv.Itoa(port)
	return url
}
