using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lobby.Models.DbConfigurations;

public class PartyConfiguration : IEntityTypeConfiguration<Party>
{
    public void Configure(EntityTypeBuilder<Party> entity)
    {
        entity.HasKey(e => e.PartyId);
        
        entity.ToTable("parties");
        
        entity.HasIndex(e => e.LeaderUserId);
        
        entity.Property(e => e.PartyId)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("party_id");

        entity.Property(e => e.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnName("created");

        entity.Property(e => e.LeaderUserId)
            .HasColumnName("leader_user_id");
        
        entity.Property(e => e.Removed)
            .HasColumnName("removed");
        
        entity.Property(e => e.Size)
            .HasColumnName("size");
        
        entity.Property(e => e.GameId)
            .HasColumnName("game_id");
        
        entity.Property(e => e.TicketId)
            .HasColumnName("ticket_id");
        
        entity.HasOne(d => d.LeaderUser)
            .WithMany(p => p.Parties)
            .HasForeignKey(d => d.LeaderUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(d => d.Game)
            .WithMany(p => p.Parties)
            .HasForeignKey(d => d.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}