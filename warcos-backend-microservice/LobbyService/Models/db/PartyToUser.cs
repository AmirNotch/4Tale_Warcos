using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class PartyToUser
{
    public Guid PartyId { get; set; }

    public Guid UserId { get; set; }

    public bool IsConfirmedPartyPartaking { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Removed { get; set; }

    [JsonIgnore]
    public virtual Party Party { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
