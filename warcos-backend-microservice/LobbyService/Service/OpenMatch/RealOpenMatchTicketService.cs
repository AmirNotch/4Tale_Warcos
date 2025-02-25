using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using OpenMatch;

using static OpenMatch.FrontendService;

namespace Lobby.Service.OpenMatch;

public class RealOpenMatchTicketService : IOpenMatchTicketService {
    private readonly ILogger<RealOpenMatchTicketService> _logger;
    private readonly string _frontendAddress;

    private static readonly string userIdsKey = "user_ids";

    public RealOpenMatchTicketService(ILogger<RealOpenMatchTicketService> logger) {
        _logger = logger;
        string? frontendAddressEnv = Environment.GetEnvironmentVariable("FRONTEND_ADDRESS") ??
            throw new ApplicationException("Environment variable FRONTEND_ADDRESS is not set!");
        _frontendAddress = frontendAddressEnv!;
    }

    public async Task<Ticket> Create(string gameModeType, IEnumerable<Guid> userIds) {
        _logger.LogInformation("Creating ticket with game mode {gameMode} and users {users}", gameModeType, userIds);
        FrontendServiceClient frontendClient = getOpenMatchFrontendClient();
        Ticket ticket = MakeTicket(gameModeType, userIds);
        CreateTicketRequest createRequest = new CreateTicketRequest { Ticket = ticket };
        Ticket createdTicket = await frontendClient.CreateTicketAsync(createRequest);
        _logger.LogInformation("Created ticket with id {id} for game mode {gameMode} and users {users}", createdTicket.Id, gameModeType, userIds);
        return createdTicket;
    }

    public async Task RemoveByTicketId(string ticketId) {
        _logger.LogInformation("Removing ticket with id {id}", ticketId);
        FrontendServiceClient frontendClient = getOpenMatchFrontendClient();
        DeleteTicketRequest deleteRequest = new DeleteTicketRequest { TicketId = ticketId };
        await frontendClient.DeleteTicketAsync(deleteRequest);
        _logger.LogInformation("Ticket with id {id} is removed", ticketId);
    }

    public async Task<Ticket> GetTicket(string ticketId) {
        _logger.LogInformation("Got ticket with id {id} is removed", ticketId);
        FrontendServiceClient frontendClient = getOpenMatchFrontendClient();
        GetTicketRequest getTicketRequest = new GetTicketRequest { TicketId = ticketId };
        Ticket ticket = await frontendClient.GetTicketAsync(getTicketRequest);
        _logger.LogInformation("Got ticket with id {id}: {ticket}", ticketId, ticket);
        return ticket;
    }

    private FrontendServiceClient getOpenMatchFrontendClient() {
        var channel = GrpcChannel.ForAddress(_frontendAddress);
        return new FrontendServiceClient(channel);
    }

    private static Ticket MakeTicket(string gameMode, IEnumerable<Guid> userIds) {
        Ticket ticket = new();
        ticket.SearchFields = new SearchFields();
        ticket.SearchFields.Tags.Add(gameMode);
        Dictionary<string, Any> extensions = [];
        IEnumerable<Value> values = from userId in userIds select Value.ForString(userId.ToString());
        var userIdsValue = Value.ForList(values.ToArray());
        extensions.Add(userIdsKey, Any.Pack(userIdsValue));
        ticket.Extensions.Add(extensions);
        return ticket;
    }
}
