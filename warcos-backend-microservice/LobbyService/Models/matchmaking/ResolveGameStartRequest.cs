using Lobby.Models.WsMessage;

namespace Lobby.Models.Matchmaking;

public class ResolveGameStartRequest : IInputMessageData
{
    public Guid GameId { get; set; }
    public bool Agreed { get; set; }
}