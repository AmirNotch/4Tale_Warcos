using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class GameToUser
{
    public Guid GameId { get; set; }

    public Guid UserId { get; set; }

    public bool IsConfirmedGameStart { get; set; }

    public int Team { get; set; }

    public int Squad { get; set; }

    public int Kills { get; set; }

    public int Deaths { get; set; }

    public int Assists { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Removed { get; set; }

    [JsonIgnore]
    public virtual Game Game { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
