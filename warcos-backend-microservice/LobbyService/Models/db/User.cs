using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class User
{
    public Guid UserId { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Removed { get; set; }

    [JsonIgnore]
    public virtual ICollection<GameToUser> GameToUsers { get; set; } = new List<GameToUser>();

    [JsonIgnore]
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();

    [JsonIgnore]
    public virtual ICollection<PartyToUser> PartyToUsers { get; set; } = new List<PartyToUser>();
}
