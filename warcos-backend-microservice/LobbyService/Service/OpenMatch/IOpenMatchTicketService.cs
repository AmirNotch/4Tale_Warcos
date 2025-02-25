using OpenMatch;

namespace Lobby.Service.OpenMatch;

public interface IOpenMatchTicketService {
    public Task<Ticket> Create(string gameModeType, IEnumerable<Guid> userIds);

    public Task RemoveByTicketId(string ticketId);
}

