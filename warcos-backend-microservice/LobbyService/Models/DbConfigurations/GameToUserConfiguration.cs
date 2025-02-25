using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class GameToUserConfiguration : IEntityTypeConfiguration<GameToUser>
{
    public void Configure(EntityTypeBuilder<GameToUser> entity)
    {
        entity.HasKey(e => new { e.GameId, e.UserId });

        entity.ToTable("game_to_user");
        
        entity.HasIndex(e => new { e.UserId, e.GameId })
            .HasDatabaseName("game_to_user_back_order_idx");
        
        entity.Property(e => e.GameId)
            .HasColumnName("game_id");
        
        entity.Property(e => e.UserId)
            .HasColumnName("user_id");
        
        entity.Property(e => e.Assists)
            .HasDefaultValue(0)
            .HasColumnName("assists");
        
        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");
        
        entity.Property(e => e.Deaths)
            .HasDefaultValue(0)
            .HasColumnName("deaths");
        
        entity.Property(e => e.IsConfirmedGameStart)
            .HasDefaultValue(false)
            .HasColumnName("is_confirmed_game_start");
        
        entity.Property(e => e.Kills)
            .HasDefaultValue(0)
            .HasColumnName("kills");
        
        entity.Property(e => e.Removed)
            .HasColumnName("removed");
        
        entity.Property(e => e.Team)
            .HasColumnName("team");
        
        entity.Property(e => e.Squad)
            .HasColumnName("squad");
        
        entity.HasOne(d => d.Game)
            .WithMany(p => p.GameToUsers)
            .HasForeignKey(d => d.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.User)
            .WithMany(p => p.GameToUsers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}