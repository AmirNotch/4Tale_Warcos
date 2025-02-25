using UserProfile.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserProfile.Models.DbConfigurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<db.UserProfile>
{
    public void Configure(EntityTypeBuilder<db.UserProfile> entity)
    {
        entity.HasKey(e => e.UserId);

        entity.ToTable("user_profiles");

        entity.Property(e => e.UserId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("user_id");

        entity.Property(e => e.LevelId)
            .IsRequired()
            .HasColumnName("level_id");

        entity.Property(e => e.ExperiencePoints)
            .HasDefaultValue(0)
            .HasColumnName("experience_points");

        entity.Property(e => e.Kills)
            .HasDefaultValue(0)
            .HasColumnName("kills");

        entity.Property(e => e.Deaths)
            .HasDefaultValue(0)
            .HasColumnName("deaths");

        entity.Property(e => e.Assists)
            .HasDefaultValue(0)
            .HasColumnName("assists");

        entity.Property(e => e.Wins)
            .HasDefaultValue(0)
            .HasColumnName("wins");

        entity.Property(e => e.Losses)
            .HasDefaultValue(0)
            .HasColumnName("losses");

        entity.Property(e => e.MatchesPlayed)
            .HasDefaultValue(0)
            .HasColumnName("matches_played");

        entity.Property(e => e.PlayTime)
            .HasDefaultValueSql("INTERVAL '0'")
            .HasColumnName("play_time");

        entity.Property(e => e.HeadshotKills)
            .HasDefaultValue(0)
            .HasColumnName("headshot_kills");

        entity.Property(e => e.MeleeKills)
            .HasDefaultValue(0)
            .HasColumnName("melee_kills");

        entity.Property(e => e.KillStreak)
            .HasDefaultValue(0)
            .HasColumnName("kill_streak");

        entity.HasOne(e => e.Level)
            .WithMany(l => l.UserProfiles)
            .HasForeignKey(e => e.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
