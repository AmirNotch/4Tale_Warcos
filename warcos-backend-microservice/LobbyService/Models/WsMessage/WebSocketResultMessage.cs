using Newtonsoft.Json;

namespace Lobby.Models.WsMessage;

public class WebSocketResultMessage
{
    [JsonProperty(Order = 0)]
    public string EventCategory { get; set; } = null!;
    [JsonProperty(Order = 1)]
    public string EventType { get; set; }=null!;
    [JsonProperty(Order = 2)]
    public string? ErrorCode { get; set; }
    [JsonProperty(Order = 3)]
    public string? ErrorMessage { get; set; }
    [JsonProperty(Order = 4)]
    public IOutputMessageData? Data { get; set; }
    [JsonProperty(Order = 5)]
    public DateTimeOffset Timestamp { get; set; }
    [JsonProperty(Order = 6)]
    public bool IsSuccess { get; set; }
}
