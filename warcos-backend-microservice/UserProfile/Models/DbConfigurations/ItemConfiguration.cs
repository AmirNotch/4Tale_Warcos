using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;
using static UserProfile.Models.Constant.UserProfileConstants;

namespace UserProfile.Models.DbConfigurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> entity)
    {
        entity.HasKey(e => e.ItemId);

        entity.ToTable("items");

        entity.Property(e => e.ItemId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("item_id");

        entity.Property(e => e.UnrealId)
            .IsRequired()
            .HasColumnName("name");

        // Связь с наградами уровня (LevelReward)
        entity.HasMany(e => e.LevelRewards)
            .WithOne(lr => lr.Item)
            .HasForeignKey(lr => lr.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с достижениями (Achievement)
        entity.HasMany(e => e.AchievementRewards)
            .WithOne(a => a.RewardItem)
            .HasForeignKey(a => a.RewardItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Seed Data
        entity.HasData(
            new Item
            {
                ItemId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                UnrealId = "combat_helmet",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("987f6543-a21b-34d2-c321-765432109876"),
                UnrealId = "high_precision_scope",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("4567d890-1234-56d7-e890-123456789012"),
                UnrealId = "armor_piercing_ammo",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                UnrealId = "advanced_grenade",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("fedc4321-ba98-76d5-c432-109876543210"),
                UnrealId = "reinforced_armor",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("650edfbf-8e64-4381-ba48-6aa5bc4b978d"),
                UnrealId = "urban_camo_skin",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("dade044f-09b6-4e60-b526-62d18844fd2a"),
                UnrealId = "desert_storm_skin",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("e941dbdf-9d4a-4130-8855-19432e42dcad"),
                UnrealId = "jungle_warfare_skin",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("b5877b11-5977-4168-b9bf-c4f1d81ae689"),
                UnrealId = "night_ops_skin",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            },
            new Item
            {
                ItemId = Guid.Parse("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"),
                UnrealId = "golden_dragon_skin",
                CreatedAt = DateTimeOffset.Parse(FixedDate)
            }
        );
    }
}
