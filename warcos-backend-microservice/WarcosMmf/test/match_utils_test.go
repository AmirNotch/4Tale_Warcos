package mmf

import (
	mmf "warcos-mmf/internal"

	"testing"

	"github.com/stretchr/testify/assert"
	"google.golang.org/protobuf/proto"
	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/structpb"

	"open-match.dev/open-match/pkg/pb"
)

func TestMakeTicketIdsForTeamSuccess(t *testing.T) {
	var tickets []*pb.Ticket = []*pb.Ticket{
		makeMultiplePlayersTicket(2, 1),
		makeMultiplePlayersTicket(1, 2),
		makeMultiplePlayersTicket(1, 3),
		makeMultiplePlayersTicket(1, 4),
		makeMultiplePlayersTicket(1, 5),
	}

	ticketIdsToTeams, err := mmf.MakeTicketIdsToTeams(tickets, 6, true)

	assert.Nilf(t, err, "Error message was not nil")
	assert.Equal(t, 5, len(ticketIdsToTeams), "Wrong ticketIdsToTeams size")
	assert.Equal(t, 1, ticketIdsToTeams["multiple_2_1"], "Wrong team for ticket")
	assert.Equal(t, 1, ticketIdsToTeams["multiple_1_2"], "Wrong team for ticket")
	assert.Equal(t, 2, ticketIdsToTeams["multiple_1_3"], "Wrong team for ticket")
	assert.Equal(t, 2, ticketIdsToTeams["multiple_1_4"], "Wrong team for ticket")
	assert.Equal(t, 2, ticketIdsToTeams["multiple_1_5"], "Wrong team for ticket")
}

func TestMakeTicketIdsForTeamFail(t *testing.T) {
	var tickets []*pb.Ticket = []*pb.Ticket{
		makeMultiplePlayersTicket(1, 1),
		makeMultiplePlayersTicket(1, 2),
		makeMultiplePlayersTicket(2, 3),
		makeMultiplePlayersTicket(1, 4),
		makeMultiplePlayersTicket(1, 5),
	}

	_, err := mmf.MakeTicketIdsToTeams(tickets, 6, true)

	assert.NotNilf(t, err, "Error message was nil")
}

func TestMakeTicketIdsForBattleRoyaleSuccess(t *testing.T) {
	var tickets []*pb.Ticket = []*pb.Ticket{
		makeSinglePlayerTicket(1),
		makeSinglePlayerTicket(2),
		makeSinglePlayerTicket(3),
		makeMultiplePlayersTicket(1, 4),
		makeMultiplePlayersTicket(1, 5),
		makeSinglePlayerTicket(6),
	}

	ticketIdsToTeams, err := mmf.MakeTicketIdsToTeams(tickets, 6, false)

	assert.Nilf(t, err, "Error message was not nil")
	assert.Equal(t, 6, len(ticketIdsToTeams), "Wrong ticketIdsToTeams size")
	assert.Equal(t, 1, ticketIdsToTeams["single_1"], "Wrong team for ticket")
	assert.Equal(t, 2, ticketIdsToTeams["single_2"], "Wrong team for ticket")
	assert.Equal(t, 3, ticketIdsToTeams["single_3"], "Wrong team for ticket")
	assert.Equal(t, 4, ticketIdsToTeams["multiple_1_4"], "Wrong team for ticket")
	assert.Equal(t, 5, ticketIdsToTeams["multiple_1_5"], "Wrong team for ticket")
	assert.Equal(t, 6, ticketIdsToTeams["single_6"], "Wrong team for ticket")
}

func TestMakeTicketIdsForBattleRoyaleFail(t *testing.T) {
	var tickets []*pb.Ticket = []*pb.Ticket{
		makeSinglePlayerTicket(1),
		makeSinglePlayerTicket(2),
		makeSinglePlayerTicket(3),
		makeMultiplePlayersTicket(2, 4),
		makeSinglePlayerTicket(5),
	}

	_, err := mmf.MakeTicketIdsToTeams(tickets, 6, false)

	assert.NotNilf(t, err, "Error message was nil")
}

func TestMakeTeamDataForSoloRegimes(t *testing.T) {
	var tickets [][]*pb.Ticket = [][]*pb.Ticket{
		{
			makeSinglePlayerTicket(1),
			makeMultiplePlayersTicket(3, 2),
			makeSinglePlayerTicket(3),
			makeMultiplePlayersTicket(1, 4),
		},
	}
	result, err := mmf.MakeMatchExtensions(tickets, false)
	assert.Nilf(t, err, "Error message was not nil")
	assert.NotNilf(t, result, "Result was nil")

	// Check user_to_team
	var userToTeamRaw structpb.Value
	err = anypb.UnmarshalTo(result["user_to_team"], &userToTeamRaw, proto.UnmarshalOptions{})
	if err != nil {
		panic(err)
	}
	userToTeam := userToTeamRaw.GetStructValue()
	assert.Equal(t, 6, len(userToTeam.Fields), "Wrong user_to_team size")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_1"], "Wrong team for user 1")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_2_1"], "Wrong team for user 2_1")
	assert.Equal(t, makeValue(3), userToTeam.Fields["user_2_2"], "Wrong team for user 2_2")
	assert.Equal(t, makeValue(4), userToTeam.Fields["user_2_3"], "Wrong team for user 2_3")
	assert.Equal(t, makeValue(5), userToTeam.Fields["user_3"], "Wrong team for user 3")
	assert.Equal(t, makeValue(6), userToTeam.Fields["user_4_1"], "Wrong team for user 4_1")

	// Check user_to_squad
	var userToSquadRaw structpb.Value
	err = anypb.UnmarshalTo(result["user_to_squad"], &userToSquadRaw, proto.UnmarshalOptions{})
	if err != nil {
		panic(err)
	}
	userToSquad := userToSquadRaw.GetStructValue()
	assert.Equal(t, 6, len(userToSquad.Fields), "Wrong user_to_squad size")
	assert.Equal(t, makeValue(1), userToSquad.Fields["user_1"], "Wrong squad for user 1")
	assert.Equal(t, makeValue(2), userToSquad.Fields["user_2_1"], "Wrong squad for user 2_1")
	assert.Equal(t, makeValue(3), userToSquad.Fields["user_2_2"], "Wrong squad for user 2_2")
	assert.Equal(t, makeValue(4), userToSquad.Fields["user_2_3"], "Wrong squad for user 2_3")
	assert.Equal(t, makeValue(5), userToSquad.Fields["user_3"], "Wrong squad for user 3")
	assert.Equal(t, makeValue(6), userToSquad.Fields["user_4_1"], "Wrong squad for user 4_1")
}

func TestMakeTeamDataForTeamRegimes(t *testing.T) {
	squads := [][]*pb.Ticket{
		{
			makeMultiplePlayersTicket(3, 1),
		},
		{
			makeMultiplePlayersTicket(2, 2),
			makeMultiplePlayersTicket(1, 3),
		},
		{
			makeSinglePlayerTicket(4),
			makeSinglePlayerTicket(5),
			makeSinglePlayerTicket(6),
		},
		{
			makeMultiplePlayersTicket(3, 7),
		},
	}
	var result, err = mmf.MakeMatchExtensions(squads, true)
	assert.Nilf(t, err, "Error message was not nil")
	assert.NotNilf(t, result, "Result was nil")

	// Check ticket_to_team
	var userToTeamRaw structpb.Value
	err = anypb.UnmarshalTo(result["user_to_team"], &userToTeamRaw, proto.UnmarshalOptions{})
	if err != nil {
		panic(err)
	}
	userToTeam := userToTeamRaw.GetStructValue()
	assert.Equal(t, 12, len(userToTeam.Fields))
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_1_1"], "Wrong team for user 1_1")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_1_2"], "Wrong team for user 1_2")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_1_3"], "Wrong team for user 1_3")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_2_1"], "Wrong team for user 2_1")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_2_2"], "Wrong team for user 2_2")
	assert.Equal(t, makeValue(1), userToTeam.Fields["user_3_1"], "Wrong team for user 3_1")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_4"], "Wrong team for user 4")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_5"], "Wrong team for user 5")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_6"], "Wrong team for user 6")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_7_1"], "Wrong team for user 7_1")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_7_2"], "Wrong team for user 7_2")
	assert.Equal(t, makeValue(2), userToTeam.Fields["user_7_3"], "Wrong team for user 7_3")

	// Check ticket_to_squad
	var ticketToSquadRaw structpb.Value
	err = anypb.UnmarshalTo(result["user_to_squad"], &ticketToSquadRaw, proto.UnmarshalOptions{})
	if err != nil {
		panic(err)
	}
	ticketToSquad := ticketToSquadRaw.GetStructValue()
	assert.Equal(t, 12, len(userToTeam.Fields))
	assert.Equal(t, makeValue(1), ticketToSquad.Fields["user_1_1"], "Wrong squad for user 1_1")
	assert.Equal(t, makeValue(1), ticketToSquad.Fields["user_1_2"], "Wrong squad for user 1_2")
	assert.Equal(t, makeValue(1), ticketToSquad.Fields["user_1_3"], "Wrong squad for user 1_3")
	assert.Equal(t, makeValue(2), ticketToSquad.Fields["user_2_1"], "Wrong squad for user 2_1")
	assert.Equal(t, makeValue(2), ticketToSquad.Fields["user_2_2"], "Wrong squad for user 2_2")
	assert.Equal(t, makeValue(2), ticketToSquad.Fields["user_3_1"], "Wrong squad for user 3_1")
	assert.Equal(t, makeValue(3), ticketToSquad.Fields["user_4"], "Wrong squad for user 4")
	assert.Equal(t, makeValue(3), ticketToSquad.Fields["user_5"], "Wrong squad for user 5")
	assert.Equal(t, makeValue(3), ticketToSquad.Fields["user_6"], "Wrong squad for user 6")
	assert.Equal(t, makeValue(4), ticketToSquad.Fields["user_7_1"], "Wrong squad for user 7_1")
	assert.Equal(t, makeValue(4), ticketToSquad.Fields["user_7_2"], "Wrong squad for user 7_2")
	assert.Equal(t, makeValue(4), ticketToSquad.Fields["user_7_3"], "Wrong squad for user 7_3")
}

func makeValue(arg int) *structpb.Value {
	a, err := structpb.NewValue(arg)
	if err != nil {
		panic(err)
	}
	return a
}
