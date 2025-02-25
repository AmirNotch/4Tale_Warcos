using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class ScheduledGameRegimeConfiguration : IEntityTypeConfiguration<ScheduledGameRegime>
{
    public void Configure(EntityTypeBuilder<ScheduledGameRegime> entity)
    {
        entity.HasKey(e => new { e.GameMode, e.IntervalStart });
        
        entity.ToTable("scheduled_game_regimes");
        
        entity.HasIndex(e => e.GameRegimeEntryId);
        
        entity.Property(e => e.GameMode)
            .HasColumnName("game_mode");
        
        entity.Property(e => e.GameRegime)
            .HasColumnName("game_regime");
        
        entity.Property(e => e.MapKind)
            .HasColumnName("map_kind");
        
        entity.Property(e => e.IntervalStart)
            .HasColumnName("interval_start");
    
        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");
    
        entity.Property(e => e.GameRegimeEntryId)
            .HasColumnName("game_regime_entry_id");
        
        entity.Property(e => e.IntervalEnd)
            .HasColumnName("interval_end");
        
        entity.HasOne(d => d.GameModeNavigation)
            .WithMany(p => p.ScheduledGameRegimes)
            .HasForeignKey(d => d.GameMode)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(d => d.GameRegimeNavigation)
            .WithMany(p => p.ScheduledGameRegimes)
            .HasForeignKey(d => d.GameRegime)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(d => d.MapKindNavigation)
            .WithMany(p => p.ScheduledGameRegimes)
            .HasForeignKey(d => d.MapKind)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(d => d.GameRegimeEntryNavigation)
            .WithMany(p => p.ScheduledGameRegimes)
            .HasForeignKey(d => d.GameRegimeEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}