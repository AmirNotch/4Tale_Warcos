using Lobby.Models.WsMessage;

namespace Lobby.Models.Matchmaking;

public class GetUpcomingGameRegimesResponse : IOutputMessageData
{
    public List<EnrichedGameRegimeDto> UpcomingGameRegimes { get; set; }
}
