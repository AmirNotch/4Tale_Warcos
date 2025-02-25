using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class MapConfiguration : IEntityTypeConfiguration<Map>
{
    public void Configure(EntityTypeBuilder<Map> entity)
    {
        entity.HasKey(e => e.MapKind);

        entity.ToTable("maps");

        entity.Property(e => e.MapKind)
            .HasColumnName("map_kind");
        
        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");
        
        // Seed Data
        entity.HasData(
            new Map { MapKind = "L_Freeport", Created = DateTimeOffset.UtcNow },
            new Map { MapKind = "L_Airport", Created = DateTimeOffset.UtcNow },
            new Map { MapKind = "L_Hotel", Created = DateTimeOffset.UtcNow }
        );
    }
}