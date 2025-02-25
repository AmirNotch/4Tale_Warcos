using System.Text.Json.Serialization;

namespace Lobby.Models.Games;

public class UserGameData
{
    public Guid GameId;
    public string? GameServerUrl;
    [JsonIgnore]
    public bool IsGameConfirmed;
}
