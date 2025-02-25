using Lobby.Models.WsMessage;

namespace Lobby.Models.Matchmaking;

public class BeginGameSearchRequest : IInputMessageData
{
    public List<Guid> UserIds { get; set; }
    public string GameMode { get; set; }
}
