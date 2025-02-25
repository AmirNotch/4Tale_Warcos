using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class PartyToUserConfiguration : IEntityTypeConfiguration<PartyToUser>
{
    public void Configure(EntityTypeBuilder<PartyToUser> entity)
    {
        entity.HasKey(e => new { e.PartyId, e.UserId });
        
        entity.ToTable("party_to_user");
        
        entity.HasIndex(e => new { e.UserId, e.PartyId });
        
        entity.Property(e => e.PartyId)
            .HasColumnName("party_id");
        
        entity.Property(e => e.UserId)
            .HasColumnName("user_id");
    
        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");
    
        entity.Property(e => e.IsConfirmedPartyPartaking)
            .HasDefaultValue(false)
            .HasColumnName("is_confirmed_party_partaking");
    
        entity.Property(e => e.Removed)
            .HasColumnName("removed");
        
        entity.HasOne(d => d.Party)
            .WithMany(p => p.PartyToUsers)
            .HasForeignKey(d => d.PartyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(d => d.User)
            .WithMany(p => p.PartyToUsers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}