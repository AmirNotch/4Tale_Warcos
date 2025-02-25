using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class UserProfileStatsHistoryConfiguration : IEntityTypeConfiguration<UserProfileStatsHistory>
{
    public void Configure(EntityTypeBuilder<UserProfileStatsHistory> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("user_profile_stats_history");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");

        entity.Property(e => e.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        entity.Property(e => e.LevelId)
            .IsRequired()
            .HasColumnName("level_id");

        entity.Property(e => e.ExperiencePoints)
            .IsRequired()
            .HasColumnName("experience_points");

        entity.Property(e => e.Kills)
            .IsRequired()
            .HasColumnName("kills");

        entity.Property(e => e.Deaths)
            .IsRequired()
            .HasColumnName("deaths");

        entity.Property(e => e.Assists)
            .IsRequired()
            .HasColumnName("assists");

        entity.Property(e => e.Wins)
            .IsRequired()
            .HasColumnName("wins");

        entity.Property(e => e.Losses)
            .IsRequired()
            .HasColumnName("losses");

        entity.Property(e => e.MatchesPlayed)
            .IsRequired()
            .HasColumnName("matches_played");

        entity.Property(e => e.PlayTime)
            .IsRequired()
            .HasColumnName("play_time");

        entity.Property(e => e.HeadshotKills)
            .IsRequired()
            .HasColumnName("headshot_kills");

        entity.Property(e => e.MeleeKills)
            .IsRequired()
            .HasColumnName("melee_kills");

        entity.Property(e => e.KillStreak)
            .IsRequired()
            .HasColumnName("kill_streak");

        entity.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");
        
        entity.HasOne(e => e.Level)
            .WithMany()
            .HasForeignKey(e => e.LevelId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.UserProfile)
            .WithMany(u => u.UserProfileStatsHistories)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Level)
            .WithMany(l => l.UserProfileStatsHistories)
            .HasForeignKey(e => e.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
