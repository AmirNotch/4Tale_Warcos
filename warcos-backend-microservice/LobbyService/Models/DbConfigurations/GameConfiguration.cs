using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> entity)
    {
        entity.HasKey(e => e.GameId);

        entity.ToTable("games");

        entity.HasIndex(e => e.GameRegimeEntryId);

        entity.Property(e => e.GameId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("game_id");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.GameServerUrl)
            .HasColumnName("game_server_url");

        entity.Property(e => e.GameRegimeEntryId)
            .HasColumnName("game_regime_entry_id");

        entity.Property(e => e.GameStatus)
            .HasDefaultValue(GameStatus.CONFIRMATION)
            .HasColumnName("game_status")
            .HasConversion<string>();
        
        entity.HasOne(d => d.GameRegimeEntry)
            .WithMany(p => p.Games)
            .HasForeignKey(d => d.GameRegimeEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}