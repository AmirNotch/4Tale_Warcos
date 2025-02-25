using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using System.Net.WebSockets;

using Lobby.Validation;
using Lobby.Models.WsMessage;
using Lobby.Models;
using static Lobby.Util.WebSocketUtils;

namespace Lobby.Service;

public class WebSocketHandler
{
    private readonly ILogger<WebSocketHandler> _logger;
    private readonly MatchmakingService _matchmakingService;
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly IValidationStorage _validationStorage;
    private readonly WarcosLobbyDbContext _dbContext;

    private readonly static string inputMessagePattern = "/([a-zA-Z0-9]+) request=(.*)";
    private readonly static string outputMessageFormat = "/{0} success={1}, response={2}";

    private readonly static string getUpcomingGameRegimesEndpoint = "getUpcomingGameRegimes";
    private readonly static string getMyPartyEndpoint = "getMyParty";
    private readonly static string beginGameSearchEndpoint = "beginGameSearch";
    private readonly static string cancelGameSearchEndpoint = "cancelGameSearch";
    private readonly static string resolveGameStartEndpoint = "resolveGameStart";

    public WebSocketHandler(ILogger<WebSocketHandler> logger, MatchmakingService matchmakingService,
                            IValidationStorage validationStorage, JsonSerializerSettings jsonSerializerSettings,
                            WarcosLobbyDbContext dbContext)
    {
        _logger = logger;
        _matchmakingService = matchmakingService;
        _jsonSerializerSettings = jsonSerializerSettings;
        _validationStorage = validationStorage;
        _dbContext = dbContext;
    }

    public async Task HandleWebSocket(WebSocket webSocket, Guid userId, CancellationToken ct)
    {
        // TODO обработка ошибок
        WebSocketConnectionManager.AddSocket(userId, webSocket);
        _logger.LogInformation("Listening for incoming messages as user {userId}", userId);
        try
        {
            await WsCycle(webSocket, userId, ct);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, $"An exception occurred during WebSocket connection");
        }
        catch (ConnectionAbortedException ex)
        {
            _logger.LogInformation(ex, $"Cannot create ws connection for user {userId}: server shutdown is planned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unknown exception.");
        }
        try
        {
            await FinalizeWsConnection(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An exception occurred while finalizing WebSocket connection");
        }

        _logger.LogInformation($"WebSocket connection closed for user {userId}");
    }

    private async Task WsCycle(WebSocket webSocket, Guid userId, CancellationToken ct)
    {
        var buffer = new Memory<byte>(new byte[1024]);
        await using var dataStream = new MemoryStream();
        while (webSocket.State == WebSocketState.Open)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }
            string? message = await ReadMessageFromWebSocket(webSocket, buffer, dataStream, CancellationToken.None);
            if (message == null)
            {
                _logger.LogInformation($"The WebSocket has been already closed");
                break;
            }
            try
            {
                await ProcessWsMessageSafe(webSocket, userId, message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cannot process WebSocket message");
            }
            _dbContext.ChangeTracker.Clear();
            _validationStorage.Clear();
            dataStream.SetLength(0);
        }
    }

    private async Task ProcessWsMessageSafe(WebSocket ws, Guid userId, string inputMessage, CancellationToken ct)
    {
        IncomingWsMessage? incomingMessage;
        try
        {
            incomingMessage = JsonConvert.DeserializeObject<IncomingWsMessage>(inputMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Cannot parse incoming message ${inputMessage}");
            incomingMessage = null;
        }
        if (incomingMessage == null)
        {
            await SendMessageToUser(ws, userId, _logger, "Error", "Error", null, ErrorCode.CannotParseMessage,
                "Cannot parse message", _jsonSerializerSettings, ct);
            return;
        }

        try
        {
            await ProcessWsMessage(ws, userId, incomingMessage, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while processing ws message from user {userId}");
            await SendMessageToUser(ws, userId, _logger, "Error", "Error", null, ErrorCode.InternalServerError,
                "Something went wrong", _jsonSerializerSettings, ct);
        }
    }

    private async Task ProcessWsMessage(WebSocket ws, Guid userId, IncomingWsMessage incomingMessage, CancellationToken ct)
    {
        string? inputData = incomingMessage.Data;
        IOutputMessageData? outputData = null;
        _logger.LogInformation($"Incoming ws message: eventType={incomingMessage.EventType}, data={inputData}, timestamp={incomingMessage.Timestamp}");
        ErrorCode? errorCode;
        string? errorMessage;
        switch (incomingMessage.EventCategory)
        {
            case EndpointCategory.Matchmaking:
                (outputData, errorCode, errorMessage) = await _matchmakingService.ProcessMessage(userId, incomingMessage.EventType, incomingMessage.Data, ct);
                break;
            default:
                errorCode = ErrorCode.WrongEndpointKind;
                errorMessage = $"Unknown event Category: {incomingMessage.EventCategory}";
                break;
        }
        if (errorCode == null)
        {
            if (outputData != null)
            {
                _logger.LogInformation($"Outgoing message: eventType={incomingMessage.EventType}, data={outputData}");
                await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), outputData, _jsonSerializerSettings, ct);
            }
            else
            {
                _logger.LogInformation($"Outgoing message: eventType={incomingMessage.EventType}");
                await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), null, _jsonSerializerSettings, ct);
            }
        }
        else
        {
            _logger.LogInformation($"Outgoing message with error: eventType={incomingMessage.EventType}, data={outputData}" +
                    $", errorCode={errorCode}, errorMessage={errorMessage}");
            await SendMessageToUser(ws, userId, _logger, incomingMessage.EventType, incomingMessage.EventCategory.ToString(), outputData, errorCode, errorMessage,
                _jsonSerializerSettings, ct);
        }
    }

    public async Task FinalizeWsConnection(Guid userId)
    {
        _logger.LogInformation("Finishing WebSocket connection for user {userId}", userId);
        try
        {
            await _matchmakingService.CancelGameSearch(userId, CancellationToken.None);
            if (_validationStorage.IsValid)
            {
                _logger.LogInformation($"Cancelled game search for user {userId} because of disconnect");
            }
            else
            {
                _logger.LogInformation($"Aborted cancel game search for user {userId}: errorCode = " +
                    $"{_validationStorage.GetError().Item1}, message = {_validationStorage.GetError().Item2}");
            }
        }
        finally
        {
            _logger.LogInformation("Removing WebSocket connection for user {userId}", userId);
            WebSocketConnectionManager.RemoveSocket(userId);
        }
    }
}