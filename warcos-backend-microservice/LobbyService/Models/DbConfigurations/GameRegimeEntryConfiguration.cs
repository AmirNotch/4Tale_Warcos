using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class GameRegimeEntryConfiguration : IEntityTypeConfiguration<GameRegimeEntry>
{
    public void Configure(EntityTypeBuilder<GameRegimeEntry> entity)
    {
        entity.HasKey(e => e.GameRegimeEntryId);

        entity.ToTable("game_regime_entries");

        entity.HasIndex(e => new { e.GameMode, e.GameRegime, e.MapKind })
            .IsUnique();
        
        entity.Property(e => e.GameRegimeEntryId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("game_regime_entry_id");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.GameMode)
            .HasColumnName("game_mode");
        
        entity.Property(e => e.GameRegime)
            .HasColumnName("game_regime");
        
        entity.Property(e => e.MapKind)
            .HasColumnName("map_kind");
        
        entity.HasOne(d => d.GameModeNavigation)
            .WithMany(p => p.GameRegimeEntries)
            .HasForeignKey(d => d.GameMode)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.GameRegimeNavigation)
            .WithMany(p => p.GameRegimeEntries)
            .HasForeignKey(d => d.GameRegime)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.MapKindNavigation)
            .WithMany(p => p.GameRegimeEntries)
            .HasForeignKey(d => d.MapKind)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Seed Data
        entity.HasData(
            new GameRegimeEntry
            {
                GameRegimeEntryId = Guid.NewGuid(),
                GameMode = "TEAM_FIGHT",
                GameRegime = "CAPTURE_POINT",
                MapKind = "L_Freeport",
                Created = DateTimeOffset.UtcNow
            },
            new GameRegimeEntry
            {
                GameRegimeEntryId = Guid.NewGuid(),
                GameMode = "TEAM_FIGHT",
                GameRegime = "CAPTURE_POINT",
                MapKind = "L_Airport",
                Created = DateTimeOffset.UtcNow
            },
            new GameRegimeEntry
            {
                GameRegimeEntryId = Guid.NewGuid(),
                GameMode = "TEAM_FIGHT",
                GameRegime = "CAPTURE_POINT",
                MapKind = "L_Hotel",
                Created = DateTimeOffset.UtcNow
            }
        );
    }
}