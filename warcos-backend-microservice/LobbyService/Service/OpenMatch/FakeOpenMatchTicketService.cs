using OpenMatch;

namespace Lobby.Service.OpenMatch;

public class FakeOpenMatchTicketService : IOpenMatchTicketService {
    public Task<Ticket> Create(string gameModeType, IEnumerable<Guid> userIds) {
        Ticket ticket = new Ticket();
        ticket.SearchFields = new SearchFields();
        ticket.SearchFields.Tags.Add(gameModeType);
        ticket.Id = Guid.NewGuid().ToString();
        return Task.FromResult(ticket);
    }

    public Task RemoveByTicketId(string ticketId) {
        return Task.CompletedTask;
    }
}
