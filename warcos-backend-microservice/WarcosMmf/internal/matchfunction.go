package mmf

import (
	"fmt"
	"log"

	"open-match.dev/open-match/pkg/matchfunction"
	"open-match.dev/open-match/pkg/pb"
)

const (
	// Match function name is just a label to distinguish
	// between matches that were created by different match functions
	MatchFunctionName = "basic-matchfunction"
	squadSize         = 3
	doubleSquadSize   = squadSize * 2
)

// Run is this match function's implementation of the gRPC call defined in api/matchfunction.proto.
func (s *MatchFunctionService) Run(req *pb.RunRequest, stream pb.MatchFunction_RunServer) error {
	// Fetch tickets for the pools specified in the Match Profile.
	log.Printf("Generating proposals for function %v", req.GetProfile().GetName())

	poolTickets, err := matchfunction.QueryPools(stream.Context(), s.queryServiceClient, req.GetProfile().GetPools())
	if err != nil {
		log.Printf("Failed to query tickets for the given pools, got %s", err.Error())
		return err
	}

	// Generate proposals.
	proposals, err := MakeMatches(req.GetProfile(), poolTickets)
	if err != nil {
		log.Printf("Failed to generate matches, got %s", err.Error())
		return err
	}

	// Stream the generated proposals back to Open Match.
	for _, proposal := range proposals {
		if err := stream.Send(&pb.RunResponse{Proposal: proposal}); err != nil {
			log.Printf("Failed to stream proposals to Open Match, got %s", err.Error())
			return err
		}
	}

	return nil
}

// This function makes matches from incoming tickets.
// A match is a combination of two teams with predetermined number of players.
// The function cycles through the 'tickets' slice and tries its best to find matches.
// It keeps track of all open teams that it found so far in attempt to form a closed team.
// When two closed teams are ready, MakeMatch ebites them together and calls it a match.
// Older tickets come first, newer - last.
//
//	NOTE:
//		This implementation is fast but not the most efficient.
//		For example, if we give it a pool of 4 tickets consisting of 1, 1, 2 and 2 correspondingly,
//		it won't find any match due to straightforwardness of the internal loop.
func MakeMatches(p *pb.MatchProfile, poolTickets map[string][]*pb.Ticket) ([]*pb.Match, error) {
	var matches []*pb.Match
	var closedGroups [][]*pb.Ticket
	count := 0
	numberOfPlayers, err := getNumberOfPlayersFromMatchProfile(p)
	if err != nil {
		return nil, err
	}
	isTeam, err := getIsTeamFromMatchProfile(p)
	if err != nil {
		return nil, err
	}

	var groupCount int
	var groupSize int
	if isTeam {
		groupCount = int(numberOfPlayers / 3) // divide all users into squads
		groupSize = squadSize
		if numberOfPlayers%doubleSquadSize != 0 {
			error, _ := fmt.Printf("Fatal error: number of players for team game mode %s is not divisible by %d", p.Name, doubleSquadSize)
			panic(error)
		}
	} else {
		groupCount = 1 // one group to gather them all
		groupSize = numberOfPlayers
	}

	for poolName, tickets := range poolTickets {
		openGroups := map[int][][]*pb.Ticket{}
		closedGroups = make([][]*pb.Ticket, 0)
		var matchTickets []*pb.Ticket = nil
		for _, ticket := range tickets {
			closedTeam, err := addNewTicket(openGroups, ticket, groupSize)
			if err != nil {
				return nil, err
			}
			if closedTeam == nil {
				continue
			}
			matchTickets = append(matchTickets, closedTeam...)
			closedGroups = append(closedGroups, closedTeam)
			if len(closedGroups) < groupCount {
				continue
			}
			match, err := makeMatch(p, matchTickets, closedGroups, poolName, count, isTeam)
			if err != nil {
				return nil, err
			}
			matches = append(matches, match)
			matchTickets = nil
			closedGroups = nil
			count++
		}
		openGroupsString, err := formatOpenGroups(openGroups)
		if err != nil {
			return nil, err
		}
		closedGroupsString, err := formatTicketGroups(closedGroups)
		if err != nil {
			return nil, err
		}
		log.Printf("%s. Incoming tickets: %v, outgoing matches: %v, open ticket groups: %v, closed ticket groups: %v",
			p.Name, len(tickets), len(matches), openGroupsString, closedGroupsString)
	}
	return matches, nil
}

func addNewTicket(openTeams map[int][][]*pb.Ticket, newTicket *pb.Ticket, groupSize int) ([]*pb.Ticket, error) {
	playerCount, err := getTicketPlayerCount(newTicket)
	if err != nil {
		return nil, err
	}
	// If ticket's player count is enough to fill the entire team then we don't need to bother with openTeams
	if playerCount >= groupSize {
		return []*pb.Ticket{newTicket}, nil
	}

	// Main cycle. Partially filled teams go first.
	// Filling a team to full size is ideal, that's why we start from ticketsPerTeam - playerCount
	for openTeamSize := groupSize - playerCount; openTeamSize > 0; openTeamSize-- {
		teams, isPresent := openTeams[openTeamSize]
		if !isPresent || len(teams) == 0 {
			continue
		}
		openTeam := teams[0]
		openTeams[openTeamSize] = openTeams[openTeamSize][1:]
		openTeam = append(openTeam, newTicket)
		newTeamSize := openTeamSize + playerCount
		if newTeamSize == groupSize {
			return openTeam, nil
		} else {
			openTeams[newTeamSize] = append(openTeams[newTeamSize], openTeam)
			return nil, nil
		}
	}
	// No complementary open team is found. Then we just create a new team with the ticket
	openTeams[playerCount] = append(openTeams[playerCount], []*pb.Ticket{newTicket})
	return nil, nil
}

func formatOpenGroups(openGroups map[int][][]*pb.Ticket) (string, error) {
	result := "{"
	isFirst := true
	for groupSize, ticketGroups := range openGroups {
		if !isFirst {
			result += ", "
		}
		isFirst = false
		ticketGroupsString, err := formatTicketGroups(ticketGroups)
		if err != nil {
			return "", err
		}
		result += fmt.Sprintf("%d: %v", groupSize, ticketGroupsString)
	}
	result += "}"
	return result, nil
}

func formatTicketGroups(ticketGroups [][]*pb.Ticket) (string, error) {
	result := "["
	for groupIdx, tickets := range ticketGroups {
		if groupIdx > 0 {
			result += ", ["
		}
		for ticketIdx, ticket := range tickets {
			if ticketIdx > 0 {
				result += ", "
			}
			playerCount, err := getTicketPlayerCount(ticket)
			if err != nil {
				return "", err
			}
			result += fmt.Sprintf("{\"id\": \"%s\", \"playerCount\": %v}", ticket.Id, playerCount)
		}
		result += "]"
	}
	result += "]"
	return result, nil
}
