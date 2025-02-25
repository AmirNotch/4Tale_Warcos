using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.UserId);
        
        entity.ToTable("users");
        
        entity.Property(e => e.UserId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("user_id");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.Removed)
            .HasColumnName("removed");
    }
}