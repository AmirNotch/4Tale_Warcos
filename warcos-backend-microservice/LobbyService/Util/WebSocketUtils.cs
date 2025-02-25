using Lobby.Models.WsMessage;
using Lobby.Validation;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Lobby.Util;

public class WebSocketUtils
{
    public static async Task SendMessageToUser(WebSocket ws,
        Guid userId,
        ILogger logger,
        string endpointKind,
        string  endpointCategory,
        IOutputMessageData? data,
        JsonSerializerSettings jsonSerializerSettings,
        CancellationToken ct)
    {
        await SendMessageToUser(ws, userId, logger, endpointKind, endpointCategory, data, null, null, jsonSerializerSettings, ct);
    }

    public static async Task SendMessageToUser(WebSocket ws,
        Guid userId,
        ILogger logger,
        string endpointKind,
        string endpointCategory,
        IOutputMessageData? data,
        ErrorCode? errorCode,
        string? error,
        JsonSerializerSettings jsonSerializerSettings, 
        CancellationToken ct)
    {
        WebSocketResultMessage result = new();
        result.Data = data;
        result.EventType = HigherToLowerCamelCase(endpointKind);
        result.EventCategory = HigherToLowerCamelCase(endpointCategory.ToString());
        result.Timestamp = DateTimeOffset.UtcNow;
        if (errorCode != null)
        {
            result.ErrorCode = errorCode.ToString();
            result.ErrorMessage = error;
            result.IsSuccess = false;
        }
        else
        {
            result.IsSuccess=true;
        }
        await SendMessageToWebSocket(ws, userId, logger, JsonConvert.SerializeObject(result, jsonSerializerSettings), ct);
    }

    public static async Task SendMessageToWebSocket(WebSocket ws, Guid userId, ILogger logger, string message, CancellationToken ct)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        if (ws.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes);
            try
            {
                logger.LogInformation($"Sending ws message {message} to user {userId}");
                await ws.SendAsync(arraySegment, messageType: WebSocketMessageType.Text, endOfMessage: true, ct);
            }
            catch (WebSocketException ex)
            {
                logger.LogError(ex, $"An exception occurred while sending ws message {message} to user {userId}");
            }
        }
    }

    public static async Task<string?> ReadMessageFromWebSocket(WebSocket ws, Memory<byte> buffer, MemoryStream dataStream, CancellationToken ct)
    {
        ValueWebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(buffer, ct);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", ct);
                return null;
            }
            await dataStream.WriteAsync(buffer[..result.Count], ct);
        } while (!result.EndOfMessage);

        return Encoding.UTF8.GetString(dataStream.ToArray());
    }

    public static async Task<string?> ReadMessageFromWebSocket(WebSocket ws)
    {
        byte[] buffer = new byte[1024];
        MemoryStream memoryStream = new();
        return await ReadMessageFromWebSocket(ws, buffer, memoryStream, CancellationToken.None);
    }

    public static string HigherToLowerCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}