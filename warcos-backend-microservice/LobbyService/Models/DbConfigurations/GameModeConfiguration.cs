using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class GameModeConfiguration : IEntityTypeConfiguration<GameMode>
{
    public void Configure(EntityTypeBuilder<GameMode> entity)
    {
        entity.HasKey(e => e.GameModeKind);

        entity.ToTable("game_modes");

        entity.Property(e => e.GameModeKind)
            .HasColumnName("game_mode_kind");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.NumberOfPlayers)
            .HasColumnName("number_of_players");

        entity.Property(e => e.IsTeam)
            .HasColumnName("is_team");
        
        // Seed Data
        entity.HasData(
            new GameMode
            {
                GameModeKind = "TEAM_FIGHT",
                NumberOfPlayers = 12,
                IsTeam = true,
                Created = DateTimeOffset.UtcNow
            }
        );
    }
}