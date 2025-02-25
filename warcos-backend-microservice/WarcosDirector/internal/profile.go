package director

import (
	"fmt"
	models "warcos-director/internal/models"

	"google.golang.org/protobuf/types/known/anypb"
	"google.golang.org/protobuf/types/known/wrapperspb"
	"open-match.dev/open-match/pkg/pb"
)

// generates profiles for the Warcos II with all possible game modes
func GenerateProfiles(gameModes []*models.GameMode) ([]*pb.MatchProfile, error) {
	var profiles []*pb.MatchProfile
	for _, gameMode := range gameModes {
		gameModeKind := gameMode.GameModeKind
		numberOfPlayers := gameMode.NumberOfPlayers
		isTeam := gameMode.IsTeam
		extensions := make(map[string]*anypb.Any)

		numberOfPlayersField, err := anypb.New(wrapperspb.Int32(numberOfPlayers))
		if err != nil {
			return nil, fmt.Errorf("failed to generate match profiles because of ghastly game mode %s, got error %w", gameModeKind, err)
		}
		extensions["number_of_players"] = numberOfPlayersField

		isTeamField, err := anypb.New(wrapperspb.Bool(isTeam))
		if err != nil {
			return nil, fmt.Errorf("failed to generate match profiles because of abysmal isTeam value %t, got error %w", isTeam, err)
		}
		extensions["is_team"] = isTeamField

		profiles = append(profiles, &pb.MatchProfile{
			Name: "match_profile_" + gameModeKind,
			Pools: []*pb.Pool{
				{
					Name: "pool_" + gameModeKind,
					TagPresentFilters: []*pb.TagPresentFilter{
						{
							Tag: gameModeKind,
						},
					},
				},
			},
			Extensions: extensions,
		})
	}
	return profiles, nil
}
