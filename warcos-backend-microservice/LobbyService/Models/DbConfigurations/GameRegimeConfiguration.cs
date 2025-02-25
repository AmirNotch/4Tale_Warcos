using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class GameRegimeConfiguration : IEntityTypeConfiguration<GameRegime>
{
    public void Configure(EntityTypeBuilder<GameRegime> entity)
    {
        entity.HasKey(e => e.GameRegimeKind);

        entity.ToTable("game_regimes");

        entity.Property(e => e.GameRegimeKind)
            .HasColumnName("game_regime_kind");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.GameMode)
            .HasColumnName("game_mode");
        
        entity.HasOne(d => d.GameModeKindNavigation)
            .WithMany(p => p.GameRegimes)
            .HasForeignKey(d => d.GameMode)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Seed Data
        entity.HasData(
            new GameRegime
            {
                GameRegimeKind = "CAPTURE_POINT",
                GameMode = "TEAM_FIGHT",
                Created = DateTimeOffset.UtcNow
            }
        );
    }
}