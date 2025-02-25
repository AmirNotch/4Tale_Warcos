using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class Map
{
    public string MapKind { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    [JsonIgnore]
    public virtual ICollection<GameRegimeEntry> GameRegimeEntries { get; set; } = new List<GameRegimeEntry>();

    [JsonIgnore]
    public virtual ICollection<ScheduledGameRegime> ScheduledGameRegimes { get; set; } = new List<ScheduledGameRegime>();
}
