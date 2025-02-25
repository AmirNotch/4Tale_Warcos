using Lobby.Models.WsMessage;

namespace Lobby.Models.Matchmaking;

public class BeginGameSearchResponse : IOutputMessageData
{
    public string TicketId { get; set; }
}
