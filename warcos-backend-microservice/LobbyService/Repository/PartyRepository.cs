using Lobby.Models;
using Lobby.Models.db;
using Lobby.Models.Parties;
using Lobby.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Lobby.Repository;

public class PartyRepository : IPartyRepository
{
    private readonly ILogger<PartyRepository> _logger;
    private readonly WarcosLobbyDbContext _dbContext;

    public PartyRepository(ILogger<PartyRepository> logger, WarcosLobbyDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Party?> GetById(Guid partyId, CancellationToken ct)
    {
        _logger.LogInformation("Getting party by id {partyId}", partyId);
        return await _dbContext.Parties.FindAsync([partyId], cancellationToken: ct);
    }

    public async Task<UserPartyData?> GetActivePartyByUserId(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Getting an active party by user id {userId}", userId);
        return await _dbContext.PartyToUsers
            .Where(p2u => p2u.UserId == userId && p2u.Removed == null)
            .Select(p2u => new UserPartyData
            {
                IsPartyConfirmed = p2u.IsConfirmedPartyPartaking,
                PartyId = p2u.PartyId,
            })
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<Party>> GetByTicketIds(IEnumerable<string> ticketIds, CancellationToken ct)
    {
        _logger.LogInformation("Getting parties by ticket ids {ticketIds}", string.Join(", ", ticketIds));
        return await _dbContext.Parties
            .Where(party => ticketIds.Contains(party.TicketId))
            .ToListAsync(ct);
    }

    public async Task<List<Guid>> FilterUsersWithExistingParties(List<Guid> userIds, CancellationToken ct)
    {
        _logger.LogInformation("Filtering users with existing parties. UserIds = {userIds}", string.Join(", ", userIds));
        return await _dbContext.PartyToUsers
            .Where(p2u => userIds.Contains(p2u.UserId) && p2u.Removed == null)
            .Select(p2u => p2u.UserId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<List<Party>> GetPartiesWithoutGames(CancellationToken ct)
    {
        _logger.LogInformation("Getting active parties without a game");
        return await _dbContext.Parties
            .Where(party => party.Removed == null && party.GameId == null)
            .ToListAsync(ct);
    }

    public async Task CreateParty(string ticketId, Guid leaderUserId, List<Guid> userIds, CancellationToken ct)
    {
        Party party = new()
        {
            PartyId = Guid.NewGuid(),
            LeaderUserId = leaderUserId,
            TicketId = ticketId,
            Size = userIds.Count
        };

        var partyToUsers = userIds.Select(userId => new PartyToUser
        {
            PartyId = party.PartyId,
            UserId = userId
        }).ToList();

        _dbContext.Parties.Add(party);
        _dbContext.PartyToUsers.AddRange(partyToUsers);

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemovePartiesByTicketIds(IEnumerable<string> ticketIds, CancellationToken ct)
    {
        var parties = await _dbContext.Parties
            .Where(party => ticketIds.Contains(party.TicketId))
            .ToListAsync(ct);

        parties.ForEach(party => party.Removed = DateTimeOffset.UtcNow);

        List<Guid> partyIds = parties.Select(party => party.PartyId).ToList();

        await RemovePartiesToUsersByPartyId(partyIds, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemovePartyById(Guid partyId, CancellationToken ct)
    {
        var party = await _dbContext.Parties
            .FirstOrDefaultAsync(p => p.PartyId == partyId, ct)
            ?? throw new ApplicationException($"Cannot find party by id {partyId}");

        party.Removed = DateTimeOffset.UtcNow;

        await RemovePartiesToUsersByPartyId(new List<Guid> { partyId }, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task CancelManyGameSearches(List<Party> parties, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        parties.ForEach(party => party.Removed = now);
        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task RemovePartiesToUsersByPartyId(List<Guid> partyIds, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var partiesToUsers = await _dbContext.PartyToUsers
            .Where(p2u => partyIds.Contains(p2u.PartyId))
            .ToListAsync(ct);

        partiesToUsers.ForEach(p2u => p2u.Removed = now);
    }

    public async Task<Guid?> GetActivePartyIdByUserId(Guid userId, CancellationToken ct)
    {
        var partyToUser = await _dbContext.PartyToUsers
            .Where(p2u => p2u.UserId == userId && p2u.Removed == null)
            .FirstOrDefaultAsync(ct);

        return partyToUser?.PartyId;
    }

    public async Task<List<Guid>> GetUserIdsByPartyId(Guid partyId, CancellationToken ct)
    {
        _logger.LogInformation("Getting user ids by party id {partyId}", partyId);

        return await _dbContext.PartyToUsers
            .Where(p2u => p2u.PartyId == partyId && p2u.Removed == null)
            .Select(p2u => p2u.UserId)
            .ToListAsync(ct);
    }

    public Dictionary<Guid, string> GetUserIdToTicketFromTickets(IEnumerable<string> ticketIds)
    {
        return _dbContext.Parties
            .Join(_dbContext.PartyToUsers, 
                  party => new { party.PartyId, Removed = (DateTimeOffset?)null }, 
                  p2u => new { p2u.PartyId, p2u.Removed }, 
                  (party, p2u) => new { p2u.UserId, party.TicketId })
            .Where(p => ticketIds.Contains(p.TicketId))
            .ToDictionary(p => p.UserId, p => p.TicketId);
    }
}