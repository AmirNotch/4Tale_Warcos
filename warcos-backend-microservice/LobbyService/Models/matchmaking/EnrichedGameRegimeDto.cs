namespace Lobby.Models.Matchmaking;

public class EnrichedGameRegimeDto
{
    public string GameMode { get; set; } = null!;

    public string GameRegime { get; set; } = null!;

    public string MapKind { get; set; } = null!;

    public Guid GameRegimeEntryId { get; set; }

    public int NumberOfPlayers { get; set; }

    public bool IsTeam { get; set; }

    public DateTimeOffset IntervalStart { get; set; }

    public DateTimeOffset IntervalEnd { get; set; }
}
