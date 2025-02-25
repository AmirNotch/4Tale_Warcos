package mmf

import (
	"errors"
	"fmt"
	"strings"
	"time"

	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/structpb"
	"google.golang.org/protobuf/types/known/wrapperspb"
	"open-match.dev/open-match/pkg/pb"
)

const (
	userToTeamLabel      = "user_to_team"
	userToSquadLabel     = "user_to_squad"
	numberOfPlayersLabel = "number_of_players"
	isTeamLabel          = "is_team"
	userIdsLabel         = "user_ids"
)

func getNumberOfPlayersFromMatchProfile(mp *pb.MatchProfile) (int, error) {
	extensions := mp.Extensions
	if extensions == nil {
		return 0, errors.New("cannot get number of players from match profile: extensions do not exist")
	}
	var numberOfPlayers wrapperspb.Int32Value
	err := anypb.UnmarshalTo(extensions[numberOfPlayersLabel], &numberOfPlayers, proto.UnmarshalOptions{})
	if err != nil {
		return 0, err
	}
	return int(numberOfPlayers.Value), nil
}

func getIsTeamFromMatchProfile(mp *pb.MatchProfile) (bool, error) {
	extensions := mp.Extensions
	if extensions == nil {
		return false, errors.New("cannot get isTeam from match profile: extensions do not exist")
	}
	var isTeam wrapperspb.BoolValue
	err := anypb.UnmarshalTo(extensions[isTeamLabel], &isTeam, proto.UnmarshalOptions{})
	if err != nil {
		return false, err
	}
	return isTeam.Value, nil
}

func makeMatch(mp *pb.MatchProfile, matchTickets []*pb.Ticket, ticketGroups [][]*pb.Ticket, poolName string, matchIndex int, isTeam bool) (*pb.Match, error) {
	extensions, err := MakeMatchExtensions(ticketGroups, isTeam)
	if err != nil {
		return nil, err
	}
	return &pb.Match{
		MatchId:       fmt.Sprintf("profile-%v-time-%v-%v", poolName, time.Now().Format("2006-01-02T15:04:05.00"), matchIndex),
		MatchProfile:  mp.Name,
		MatchFunction: MatchFunctionName,
		Tickets:       matchTickets,
		Extensions:    extensions,
	}, nil
}

func MakeMatchExtensions(squads [][]*pb.Ticket, isTeam bool) (map[string]*anypb.Any, error) {
	var extensions = map[string]*anypb.Any{}
	teamMap := map[string]interface{}{}
	squadMap := map[string]interface{}{}

	team := 1
	for squadIndex, squad := range squads {
		for _, ticket := range squad {
			userIds, err := getUserIdsFromTicket(ticket)
			if err != nil {
				return nil, err
			}
			for _, userId := range userIds {
				if isTeam {
					isFirstTeam := squadIndex < len(squads)/2
					squadMap[userId] = squadIndex + 1
					if isFirstTeam {
						teamMap[userId] = 1
					} else {
						teamMap[userId] = 2
					}
				} else {
					teamMap[userId] = team
					squadMap[userId] = team
					team++
				}
			}
		}
	}
	teamMapStruct, err := structpb.NewValue(teamMap)
	if err != nil {
		return nil, err
	}
	userToTeam, err := anypb.New(teamMapStruct)
	if err != nil {
		return nil, err
	}
	squadMapStruct, err := structpb.NewValue(squadMap)
	if err != nil {
		return nil, err
	}
	userToSquad, err := anypb.New(squadMapStruct)
	if err != nil {
		return nil, err
	}
	extensions[userToTeamLabel] = userToTeam
	extensions[userToSquadLabel] = userToSquad
	return extensions, nil
}

func MakeTicketIdsToTeams(tickets []*pb.Ticket, numberOfPlayers int, isTeam bool) (map[string]int, error) {
	var playersPerTeam int = 1
	if isTeam {
		playersPerTeam = numberOfPlayers / 2
	}
	ticketIdsToTeams := make(map[string]int)
	var playersInCurrentTeam int = 0
	var currentTeamIndex int32 = 1
	for _, ticket := range tickets {
		ticketPlayerCount, err := getTicketPlayerCount(ticket)
		if err != nil {
			return nil, err
		}
		playersInCurrentTeam += ticketPlayerCount
		if playersInCurrentTeam > playersPerTeam {
			ticketsString := ticketArrayToString(tickets)
			return nil, fmt.Errorf("incorrect tickets in the match, couldn't form ticketIdsToTeams! Tickets: %v, number of players %d, isTeam %t",
				ticketsString, numberOfPlayers, isTeam)
		} else if playersInCurrentTeam == playersPerTeam {
			ticketIdsToTeams[ticket.Id] = int(currentTeamIndex)
			playersInCurrentTeam = 0
			currentTeamIndex++
		} else {
			ticketIdsToTeams[ticket.Id] = int(currentTeamIndex)
		}
	}
	return ticketIdsToTeams, nil
}

func getTicketPlayerCount(ticket *pb.Ticket) (int, error) {
	userIds, err := getUserIdsFromTicket(ticket)
	if err != nil {
		return 0, err
	}
	return len(userIds), nil
}

func getUserIdsFromTicket(ticket *pb.Ticket) ([]string, error) {
	extensions := ticket.Extensions
	if extensions == nil {
		return nil, errors.New("cannot get userIds from ticket: extensions do not exist")
	}
	var userIdsProto structpb.Value
	err := anypb.UnmarshalTo(extensions[userIdsLabel], &userIdsProto, proto.UnmarshalOptions{})
	if err != nil {
		return nil, err
	}

	result := make([]string, 0)
	for _, userIdProto := range userIdsProto.GetListValue().Values {
		userId := userIdProto.GetStringValue()
		if userId == "" {
			return nil, fmt.Errorf("empty userId in ticket %s", ticket.Id)
		}
		result = append(result, userId)
	}
	return result, nil
}

func ticketArrayToString(tickets []*pb.Ticket) string {
	var result []string
	for _, ticket := range tickets {
		ticketsb, err := proto.Marshal(ticket)
		if err != nil {
			fmt.Printf("Can not marshal ticket %s", ticket.Id)
		} else {
			result = append(result, string(ticketsb[:]))
		}
	}
	return "[" + strings.Join(result, ",") + "]"
}
