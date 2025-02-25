using Lobby.Models.WsMessage;

namespace Lobby.Models.Matchmaking;

public class GetUpcomingGameRegimesRequest : IInputMessageData
{
    public string GameMode { get; set; }
}
