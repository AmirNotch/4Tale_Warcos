namespace Lobby.Models.Matchmaking;

public class StartGameRequest
{
    public Dictionary<Guid, int> UserToTeam { get; set; }
    public Dictionary<Guid, int> UserToSquad { get; set; }
    public List<string> TicketIds { get; set; }
    public Guid GameRegimeEntryId { get; set; }
    public string GameServerUrl { get; set; }
}