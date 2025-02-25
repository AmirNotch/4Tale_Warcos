using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Lobby.Service;

public class WebSocketConnectionManager
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> _sockets = [];

    public static void AddSocket(Guid userId, WebSocket socket)
    {
        _sockets[userId] = socket;
    }

    public static WebSocket? GetSocketByUserId(Guid userId)
    {
        return _sockets.TryGetValue(userId, out var teamSockets) ? teamSockets : null;
    }

    public static void RemoveSocket(Guid userId)
    {
        _sockets.Remove(userId, out _);
    }

    public static IEnumerable<Guid> GetConnectedUserIds()
    {
        return _sockets.Keys;
    }
}