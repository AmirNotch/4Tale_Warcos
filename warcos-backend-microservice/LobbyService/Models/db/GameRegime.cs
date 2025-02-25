using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class GameRegime
{
    public string GameRegimeKind { get; set; } = null!;

    public string GameMode { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    [JsonIgnore]
    public virtual GameMode GameModeKindNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<GameRegimeEntry> GameRegimeEntries { get; set; } = new List<GameRegimeEntry>();

    [JsonIgnore]
    public virtual ICollection<ScheduledGameRegime> ScheduledGameRegimes { get; set; } = new List<ScheduledGameRegime>();
}
