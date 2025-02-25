using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfile.Models.db;

namespace UserProfile.Models.DbConfigurations;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> entity)
    {
        entity.HasKey(e => e.LevelId);

        entity.ToTable("levels");

        entity.Property(e => e.LevelId)
            .HasColumnName("level");

        entity.Property(e => e.ExperiencePoints)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("experience_points");
        
        // Seed Data
        entity.HasData(
            new Level { LevelId = 1, ExperiencePoints = 1000 },
            new Level { LevelId = 2, ExperiencePoints = 2000 },
            new Level { LevelId = 3, ExperiencePoints = 4000 },
            new Level { LevelId = 4, ExperiencePoints = 7000 },
            new Level { LevelId = 5, ExperiencePoints = 10000 },
            new Level { LevelId = 6, ExperiencePoints = 13000 },
            new Level { LevelId = 7, ExperiencePoints = 18000 },
            new Level { LevelId = 8, ExperiencePoints = 23000 },
            new Level { LevelId = 9, ExperiencePoints = 29000 },
            new Level { LevelId = 10, ExperiencePoints = 35000 }
            );
    }
}
