using Lobby.Models.db;
using Lobby.Models.Parties;

namespace Lobby.Repository.Interface;

public interface IPartyRepository
{
    Task<Party?> GetById(Guid partyId, CancellationToken ct);
    Task<UserPartyData?> GetActivePartyByUserId(Guid userId, CancellationToken ct);
    Task<List<Party>> GetByTicketIds(IEnumerable<string> ticketIds, CancellationToken ct);
    Task<List<Guid>> FilterUsersWithExistingParties(List<Guid> userIds, CancellationToken ct);
    Task<List<Party>> GetPartiesWithoutGames(CancellationToken ct);
    Task CreateParty(string ticketId, Guid leaderUserId, List<Guid> userIds, CancellationToken ct);
    Task RemovePartiesByTicketIds(IEnumerable<string> ticketIds, CancellationToken ct);
    Task RemovePartyById(Guid partyId, CancellationToken ct);
    Task CancelManyGameSearches(List<Party> parties, CancellationToken ct);
    Task<Guid?> GetActivePartyIdByUserId(Guid userId, CancellationToken ct);
    Task<List<Guid>> GetUserIdsByPartyId(Guid partyId, CancellationToken ct);
    Dictionary<Guid, string> GetUserIdToTicketFromTickets(IEnumerable<string> ticketIds);
}
