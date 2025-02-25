using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class Party
{
    public Guid PartyId { get; set; }

    public Guid LeaderUserId { get; set; }

    public string TicketId { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Removed { get; set; }

    public int Size { get; set; }

    public Guid? GameId { get; set; }

    [JsonIgnore]
    public virtual User LeaderUser { get; set; } = null!;

    [JsonIgnore]
    public virtual Game Game { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<PartyToUser> PartyToUsers { get; set; } = new List<PartyToUser>();
}
