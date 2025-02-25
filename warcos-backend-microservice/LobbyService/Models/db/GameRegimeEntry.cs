using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class GameRegimeEntry
{
    public Guid GameRegimeEntryId { get; set; }

    public string GameMode { get; set; } = null!;

    public string GameRegime { get; set; } = null!;

    public string MapKind { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    [JsonIgnore]
    public virtual GameMode GameModeNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual GameRegime GameRegimeNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    [JsonIgnore]
    public virtual Map MapKindNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ScheduledGameRegime> ScheduledGameRegimes { get; set; } = new List<ScheduledGameRegime>();
}
