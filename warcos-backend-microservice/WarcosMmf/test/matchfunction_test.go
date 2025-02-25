package mmf

import (
	mmf "warcos-mmf/internal"

	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/structpb"
	"google.golang.org/protobuf/types/known/wrapperspb"
	"open-match.dev/open-match/pkg/pb"
)

const (
	matchProfileName       = "testProfile"
	numberOfPlayers  int32 = 6
)

// The most basic test of MakeMatch func.
// All 7 incoming tickets correspond to one player.
// We expect to receive one match with the first 6 tickets
func TestMakeMatchSingle(t *testing.T) {
	matchProfile := makeMatchProfile()
	poolTickets := map[string][]*pb.Ticket{
		"pool1": {
			makeSinglePlayerTicket(1),
			makeSinglePlayerTicket(2),
			makeSinglePlayerTicket(3),
			makeSinglePlayerTicket(4),
			makeSinglePlayerTicket(5),
			makeSinglePlayerTicket(6),
			makeSinglePlayerTicket(7),
		},
	}
	matches, err := mmf.MakeMatches(matchProfile, poolTickets)

	assert.Nilf(t, err, "Error message was not nil")
	checkMatches(t, matches, [][]string{
		{"single_1", "single_2", "single_3", "single_4", "single_5", "single_6"},
	})
}

// A test of the MakeMatch func.
// All 7 incoming tickets correspond to 2 players, so they are unable to form any teams of 3 players.
// We expect no matches here.
func TestMakeMatchDouble(t *testing.T) {
	matchProfile := makeMatchProfile()
	poolTickets := map[string][]*pb.Ticket{
		"pool1": {
			makeMultiplePlayersTicket(2, 1),
			makeMultiplePlayersTicket(2, 2),
			makeMultiplePlayersTicket(2, 3),
			makeMultiplePlayersTicket(2, 4),
			makeMultiplePlayersTicket(2, 5),
			makeMultiplePlayersTicket(2, 6),
			makeMultiplePlayersTicket(2, 7),
		},
	}
	matches, err := mmf.MakeMatches(matchProfile, poolTickets)

	assert.Nilf(t, err, "Error message was not nil")
	checkMatches(t, matches, [][]string{})
}

// A test of the MakeMatch func.
// All 7 incoming tickets correspond to 3 players, so every ticket is a team by itself.
// We expect first 6 tickets to form 3 matches with pairs 1-2, 3-4, 5-6.
func TestMakeMatchTriple(t *testing.T) {
	matchProfile := makeMatchProfile()
	poolTickets := map[string][]*pb.Ticket{
		"pool1": {
			makeMultiplePlayersTicket(3, 1),
			makeMultiplePlayersTicket(3, 2),
			makeMultiplePlayersTicket(3, 3),
			makeMultiplePlayersTicket(3, 4),
			makeMultiplePlayersTicket(3, 5),
			makeMultiplePlayersTicket(3, 6),
			makeMultiplePlayersTicket(3, 7),
		},
	}
	matches, err := mmf.MakeMatches(matchProfile, poolTickets)

	assert.Nilf(t, err, "Error message was not nil")
	checkMatches(t, matches, [][]string{
		{"multiple_3_1", "multiple_3_2"},
		{"multiple_3_3", "multiple_3_4"},
		{"multiple_3_5", "multiple_3_6"},
	})
}

// A test of the MakeMatch func.
// Tickets are given in 2 separate pools.
// We expect 2 separate matches: one from each pool.
func TestMakeMatchWithSeparatePools(t *testing.T) {
	matchProfile := makeMatchProfile()
	poolTickets := map[string][]*pb.Ticket{
		"pool1": {
			makeMultiplePlayersTicket(3, 1),
			makeMultiplePlayersTicket(3, 2),
			makeMultiplePlayersTicket(3, 3),
		},
		"pool2": {
			makeMultiplePlayersTicket(3, 4),
			makeMultiplePlayersTicket(3, 5),
			makeMultiplePlayersTicket(3, 6),
		},
	}
	matches, err := mmf.MakeMatches(matchProfile, poolTickets)

	assert.Nilf(t, err, "Error message was not nil")
	checkMatches(t, matches, [][]string{
		{"multiple_3_1", "multiple_3_2"},
		{"multiple_3_4", "multiple_3_5"},
	})
}

// An ultimate test of the MakeMatch func.
// The incoming tickets have various amounts of players.
// We expect that we receive as many matches as we can make (3 in this example)
func TestMakeMatchMixed(t *testing.T) {
	matchProfile := makeMatchProfile()
	poolTickets := map[string][]*pb.Ticket{
		"pool1": {
			makeMultiplePlayersTicket(2, 1),
			makeMultiplePlayersTicket(2, 2),
			makeMultiplePlayersTicket(3, 3),
			makeMultiplePlayersTicket(2, 4),
			makeMultiplePlayersTicket(1, 5),
			makeMultiplePlayersTicket(1, 6),
			makeSinglePlayerTicket(7), // Same as makeMultiplePlayersTicket(1, 7),
			makeMultiplePlayersTicket(1, 8),
			makeMultiplePlayersTicket(1, 9),
			makeMultiplePlayersTicket(1, 10),
			makeMultiplePlayersTicket(1, 11),
			makeMultiplePlayersTicket(3, 12),
		},
	}
	matches, err := mmf.MakeMatches(matchProfile, poolTickets)

	assert.Nilf(t, err, "Error message was not nil")
	checkMatches(t, matches, [][]string{
		// The first closed team happened to be a ticket N3.
		// The second closed team is a combination of 1st 2-ticket (N1) and 1st 1-ticket (N5)
		{"multiple_3_3", "multiple_2_1", "multiple_1_5"},
		// The third closed team is a combination of 2nd 2-ticket (N2) and 2nd 1-ticket (N6)
		// The fourth closed team is a combination of 3rd 2-ticket (N4) and 3rd 1-ticket (N7)
		{"multiple_2_2", "multiple_1_6", "multiple_2_4", "single_7"},
		// The fifth closed team is a bunch of single tickets (N8, N9, N10)
		// The sixth closed team is a ticket N12.
		{"multiple_1_8", "multiple_1_9", "multiple_1_10", "multiple_3_12"},
	})
}

/************************************************************************************************/

// Check matches returned by the MakeMatch func
func checkMatches(t *testing.T, actualMatches []*pb.Match, expectedIds [][]string) {
	assert.Equal(t, len(expectedIds), len(actualMatches), "Wrong number of matches")
	for match_index, match := range actualMatches {
		assert.Equal(t, matchProfileName, match.MatchProfile, "Wrong match profile name")
		assert.Equal(t, mmf.MatchFunctionName, match.MatchFunction, "Wrong match function name")
		ticketIds := expectedIds[match_index]
		for ticket_index, ticket := range match.GetTickets() {
			assert.Equal(t, ticketIds[ticket_index], ticket.Id, "Wrong ticket id")
		}
	}
}

// Make a test ticket without playerCount field configured (playerCount = 1 by default)
func makeSinglePlayerTicket(id int32) *pb.Ticket {
	userId := structpb.NewStringValue(fmt.Sprintf("user_%d", id))
	userIds := structpb.ListValue{Values: []*structpb.Value{userId}}
	userIdsAny, err := anypb.New(&userIds)
	if err != nil {
		panic(err)
	}
	return &pb.Ticket{Id: fmt.Sprintf("single_%d", id), Extensions: map[string]*anypb.Any{
		"user_ids": userIdsAny,
	}}
}

// Make a test ticket with playerCount field configured
func makeMultiplePlayersTicket(playerCount int32, id int32) *pb.Ticket {
	list := []*structpb.Value{}
	for i := 0; i < int(playerCount); i++ {
		list = append(list, structpb.NewStringValue(fmt.Sprintf("user_%d_%d", id, i+1)))
	}
	userIds := structpb.ListValue{Values: list}
	userIdsAny, err := anypb.New(&userIds)
	if err != nil {
		panic(err)
	}
	// multiplePlayersExtension := &anypb.Any{}
	// anypb.MarshalFrom(multiplePlayersExtension, &multiplePlayers, proto.MarshalOptions{})
	return &pb.Ticket{Id: fmt.Sprintf("multiple_%d_%d", playerCount, id), Extensions: map[string]*anypb.Any{
		"user_ids": userIdsAny,
	}}
}

func makeMatchProfile() *pb.MatchProfile {
	mp := &pb.MatchProfile{Name: matchProfileName}
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
