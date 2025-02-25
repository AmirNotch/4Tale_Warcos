using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class ScheduledGameRegime
{
    public string GameMode { get; set; } = null!;

    public string GameRegime { get; set; } = null!;

    public string MapKind { get; set; } = null!;

    public Guid GameRegimeEntryId { get; set; }

    public DateTimeOffset IntervalStart { get; set; }

    public DateTimeOffset IntervalEnd { get; set; }

    public DateTimeOffset Created { get; set; }

    [JsonIgnore]
    public virtual GameMode GameModeNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual GameRegime GameRegimeNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Map MapKindNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual GameRegimeEntry GameRegimeEntryNavigation { get; set; } = null!;
}
