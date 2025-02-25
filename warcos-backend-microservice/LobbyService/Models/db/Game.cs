using System.Text.Json.Serialization;

namespace Lobby.Models.db;

public class Game
{
    public Guid GameId { get; set; }

    public string GameServerUrl { get; set; } = null!;

    public Guid GameRegimeEntryId { get; set; }

    public DateTimeOffset Created { get; set; }

    public GameStatus GameStatus { get; set; }

    [JsonIgnore]
    public virtual GameRegimeEntry GameRegimeEntry { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();

    [JsonIgnore]
    public virtual ICollection<GameToUser> GameToUsers { get; set; } = new List<GameToUser>();
}

public enum GameStatus {
    CONFIRMATION,
    IN_PROGRESS,
    FINISHED,
    CANCELLED
}
