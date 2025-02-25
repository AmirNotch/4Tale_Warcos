using System.Text.Json.Serialization;

namespace Lobby.Models.Parties;

public class UserPartyData
{
    public Guid PartyId { get; set; }
    [JsonIgnore]
    public bool IsPartyConfirmed { get; set; }
}
