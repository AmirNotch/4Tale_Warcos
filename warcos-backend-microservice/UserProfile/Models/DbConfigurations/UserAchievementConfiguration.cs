using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> entity)
    {
        entity.HasKey(e => new { e.UserId, e.AchievementId });

        entity.ToTable("user_achievements");

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");

        entity.Property(e => e.AchievementId)
            .HasColumnName("achievement_id");

        entity.Property(e => e.AchievedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("achieved_at");

        // Связь с UserProfile
        entity.HasOne(e => e.UserProfile)
            .WithMany(up => up.UserAchievements)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с Achievement
        entity.HasOne(e => e.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(e => e.AchievementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
