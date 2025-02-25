package director

import (
	"testing"
	models "warcos-director/internal/models"

	"github.com/stretchr/testify/assert"
	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/structpb"
	"google.golang.org/protobuf/types/known/wrapperspb"
	"open-match.dev/open-match/pkg/pb"
)

const (
	matchProfileName        = "testProfile"
	matchId                 = "match007"
	gameMode                = "TEAM_FIGHT"
	gameRegime              = "SWORD_CRAWL"
	mapKind                 = "CONSERVATORY"
	GameRegimeEntryId       = "abe39093-dd92-4b96-a49f-24921910b3f8"
	isTeam                  = true
	numberOfPlayers   int32 = 6
)

var ticketIds = []string{"ticket1", "ticket2", "ticket3"}
var teamMap = map[string]interface{}{
	"user1": 1,
	"user2": 1,
	"user3": 1,
	"user4": 2,
	"user5": 2,
	"user6": 2,
}

// The most basic test that check that there is no error or panic
func TestGenerateMatchProfiles(t *testing.T) {
	var gameModes []*models.GameMode
	gameMode := models.GameMode{
		GameModeKind:    "BATTLE_ROYALE",
		NumberOfPlayers: 30,
	}
	gameModes = append(gameModes, &gameMode)
	profiles, err := GenerateProfiles(gameModes)
	assert.Nilf(t, err, "Error is not nil")
	assert.Equal(t, 1, len(profiles), "Unexpected amount of match profiles")
}

func TestParseMapFromMatchExtensions(t *testing.T) {
	match := makeMatch()
	result, err := ParseMapFromMatchExtensions(match, "user_to_team")
	assert.Nilf(t, err, "Error is not nil")
	assert.Equal(t, 6, len(result), "Unexpected size of extension map")
	for userId, team := range result {
		assert.Equal(t, teamMap[userId], team, "Unexpected value in the extension map")
	}
}

// The most basic test that check that there is no error or panic
func TestAssign(t *testing.T) {
	isDevelopment = true
	mockBe := pb.BackendServiceClient(nil)
	callAssignTickets = func(be pb.BackendServiceClient, ticketIDs []string, gameServerUrl string) error {
		return nil
	}
	callStartGame = func(
		gameRegimeEntryId string, gameServerUrl string, userToTeam map[string]int, userToSquad map[string]int, ticketIDs []string,
	) error {
		return nil
	}

	getCurrentGameRegime = func(gameMode string) (*models.ScheduledGameRegime, error) {
		return &models.ScheduledGameRegime{
			GameMode:          gameMode,
			GameRegime:        gameRegime,
			MapKind:           mapKind,
			GameRegimeEntryId: GameRegimeEntryId,
		}, nil
	}

	matchProfile := makeMatchProfile()
	match := makeMatch()
	err := Assign(mockBe, match, matchProfile)
	assert.Nilf(t, err, "Error is not nil")
}

func makeMatch() *pb.Match {
	var extensions = map[string]*anypb.Any{}
	squadMap := teamMap
	tickets := []*pb.Ticket{}
	for _, id := range ticketIds {
		tickets = append(tickets, makeTicket(id))
	}

	teamMapStruct, err := structpb.NewValue(teamMap)
	if err != nil {
		panic(err)
	}
	userToTeam, err := anypb.New(teamMapStruct)
	if err != nil {
		panic(err)
	}
	squadMapStruct, err := structpb.NewValue(squadMap)
	if err != nil {
		panic(err)
	}
	userToSquad, err := anypb.New(squadMapStruct)
	if err != nil {
		panic(err)
	}
	extensions[userToTeamLabel] = userToTeam
	extensions[userToSquadLabel] = userToSquad
	return &pb.Match{
		MatchId:      matchId,
		MatchProfile: matchProfileName,
		Extensions:   extensions,
		Tickets:      tickets,
	}
}

func makeTicket(id string) *pb.Ticket {
	return &pb.Ticket{
		Id: id,
	}
}

func makeMatchProfile() *pb.MatchProfile {
	mp := &pb.MatchProfile{
		Name: matchProfileName,
		Pools: []*pb.Pool{
			{
				Name: "test_pool",
				TagPresentFilters: []*pb.TagPresentFilter{
					{
						Tag: "TEAM_FIGHT",
					},
				},
			},
		},
	}
	extensions := make(map[string]*anypb.Any)
	playerCount, err := anypb.New(wrapperspb.Int32(numberOfPlayers))
	if err != nil {
		panic(err)
	}
	isTeam, err := anypb.New(wrapperspb.Bool(true))
	if err != nil {
		panic(err)
	}
	extensions["number_of_players"] = playerCount
	extensions["is_team"] = isTeam
	mp.Extensions = extensions
	return mp
}
