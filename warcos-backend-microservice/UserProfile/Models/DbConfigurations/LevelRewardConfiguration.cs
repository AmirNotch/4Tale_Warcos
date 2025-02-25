using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class LevelRewardConfiguration : IEntityTypeConfiguration<LevelReward>
{
    public void Configure(EntityTypeBuilder<LevelReward> entity)
    {
        entity.HasKey(e => e.RewardId);

        entity.ToTable("level_rewards");

        entity.Property(e => e.LevelId)
            .HasColumnName("level_id");

        // Поле reward_type (тип награды)
        entity.Property(e => e.RewardType)
            .IsRequired()
            .HasColumnName("reward_type")
            .HasConversion<int>();

        // Поле reward_name (название награды)
        entity.Property(e => e.RewardName)
            .IsRequired()
            .HasColumnName("reward_name");

        entity.Property(e => e.ItemId)
            .IsRequired()
            .HasColumnName("item_id");

        entity.HasOne(e => e.Item)
            .WithMany(i => i.LevelRewards)
            .HasForeignKey(e => e.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        entity.HasOne(e => e.Level)
            .WithMany(l => l.LevelRewards)
            .HasForeignKey(e => e.LevelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Data for skins
        entity.HasData(
            new LevelReward
            {
                RewardId = Guid.NewGuid(),
                LevelId = 1,
                RewardType = RewardType.Skin,
                RewardName = "Urban Camo Skin",
                ItemId = Guid.Parse("650edfbf-8e64-4381-ba48-6aa5bc4b978d") // urban_camo_skin
            },
            new LevelReward
            {
                RewardId = Guid.NewGuid(),
                LevelId = 2,
                RewardType = RewardType.Skin,
                RewardName = "Desert Storm Skin",
                ItemId = Guid.Parse("dade044f-09b6-4e60-b526-62d18844fd2a") // desert_storm_skin
            },
            new LevelReward
            {
                RewardId = Guid.NewGuid(),
                LevelId = 3,
                RewardType = RewardType.Skin,
                RewardName = "Jungle Warfare Skin",
                ItemId = Guid.Parse("e941dbdf-9d4a-4130-8855-19432e42dcad") // jungle_warfare_skin
            },
            new LevelReward
            {
                RewardId = Guid.NewGuid(),
                LevelId = 4,
                RewardType = RewardType.Skin,
                RewardName = "Night Ops Skin",
                ItemId = Guid.Parse("b5877b11-5977-4168-b9bf-c4f1d81ae689") // night_ops_skin
            },
            new LevelReward
            {
                RewardId = Guid.NewGuid(),
                LevelId = 5,
                RewardType = RewardType.Skin,
                RewardName = "Golden Dragon Skin",
                ItemId = Guid.Parse("cae5c7ba-8b41-44ba-86c6-ac13731f8acc") // golden_dragon_skin
            }
        );
    }
}

