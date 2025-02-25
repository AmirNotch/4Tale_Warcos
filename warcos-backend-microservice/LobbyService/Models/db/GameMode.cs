using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class GameMode
{
    public string GameModeKind { get; set; } = null!;

    public bool IsTeam { get; set; }

    public int NumberOfPlayers { get; set; }

    public DateTimeOffset Created { get; set; }

    [JsonIgnore]
    public virtual ICollection<GameRegimeEntry> GameRegimeEntries { get; set; } = new List<GameRegimeEntry>();

    [JsonIgnore]
    public virtual ICollection<GameRegime> GameRegimes { get; set; } = new List<GameRegime>();

    [JsonIgnore]
    public virtual ICollection<ScheduledGameRegime> ScheduledGameRegimes { get; set; } = new List<ScheduledGameRegime>();
}
