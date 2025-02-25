using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class UserLevelRewardConfiguration : IEntityTypeConfiguration<UserLevelReward>
{
    public void Configure(EntityTypeBuilder<UserLevelReward> entity)
    {
        entity.HasKey(e => new { e.UserId, e.RewardId });

        entity.ToTable("users_level_rewards");

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");
        
        entity.Property(e => e.RewardId)
            .HasColumnName("reward_id");

        entity.Property(e => e.ReceivedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("received_at");
        
        entity.HasOne(e => e.UserProfile)
            .WithMany(u => u.UserLevelRewards)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.LevelReward)
            .WithMany(lr => lr.UserLevelRewards)
            .HasForeignKey(e => e.RewardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
