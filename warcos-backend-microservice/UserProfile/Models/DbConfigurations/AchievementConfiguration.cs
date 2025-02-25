using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class AchievementConfiguration : IEntityTypeConfiguration<db.Achievement>
{
    public void Configure(EntityTypeBuilder<db.Achievement> entity)
    {
        entity.HasKey(e => e.AchievementId);

        entity.ToTable("achievements");

        entity.Property(e => e.AchievementId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("achievement_id");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name");

        entity.Property(e => e.RequirementType)
            .HasColumnName("requirement_type")
            .HasConversion<int>();

        entity.Property(e => e.RequirementValue)
            .IsRequired()
            .HasColumnName("requirement_value");

        entity.Property(e => e.RewardType)
            .HasColumnName("reward_type")
            .HasConversion<int>();

        entity.Property(e => e.RewardItemId)
            .IsRequired()
            .HasColumnName("reward_item_id");

        entity.HasOne(e => e.RewardItem)
            .WithMany(i => i.AchievementRewards)
            .HasForeignKey(e => e.RewardItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Seed Data
        entity.HasData(
            new db.Achievement
            {
                AchievementId = Guid.NewGuid(),
                Name = "First Blood",
                RequirementType = RequirementType.Kills,
                RequirementValue = 1,
                RewardType = RewardType.Item,
                RewardItemId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000")
            },
            new db.Achievement
            {
                AchievementId = Guid.NewGuid(),
                Name = "Sharp Shooter",
                RequirementType = RequirementType.HeadshotKills,
                RequirementValue = 10,
                RewardType = RewardType.Item,
                RewardItemId = Guid.Parse("987f6543-a21b-34d2-c321-765432109876")
            },
            new db.Achievement
            {
                AchievementId = Guid.NewGuid(),
                Name = "Unstoppable",
                RequirementType = RequirementType.KillStreak,
                RequirementValue = 5,
                RewardType = RewardType.Item,
                RewardItemId = Guid.Parse("4567d890-1234-56d7-e890-123456789012")
            },
            new db.Achievement
            {
                AchievementId = Guid.NewGuid(),
                Name = "Master Tactician",
                RequirementType = RequirementType.Wins,
                RequirementValue = 50,
                RewardType = RewardType.Item,
                RewardItemId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890")
            },
            new db.Achievement
            {
                AchievementId = Guid.NewGuid(),
                Name = "Marathon Runner",
                RequirementType = RequirementType.PlayTime,
                RequirementValue = 1440, // Example for 24 hours in minutes
                RewardType = RewardType.Item,
                RewardItemId = Guid.Parse("fedc4321-ba98-76d5-c432-109876543210")
            }
        );

    }
}
