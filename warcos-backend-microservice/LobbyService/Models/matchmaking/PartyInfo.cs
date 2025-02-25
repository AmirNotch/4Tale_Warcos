using Lobby.Models.WsMessage;
using System.Text.Json.Serialization;

namespace Lobby.Models.Matchmaking;

public class PartyInfo : IOutputMessageData
{
    public Guid? PartyId;
    // There is no party confirmation currently
    [JsonIgnore]
    public bool IsPartyConfirmed;
    public Guid? GameId;
    public string? GameServerUrl;
    // DisableConfirmation: game confirmation is disabled
    [JsonIgnore]
    public bool IsGameConfirmed;
}
