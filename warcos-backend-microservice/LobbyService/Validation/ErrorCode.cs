namespace Lobby.Validation;

public enum ErrorCode
{    
    // Common
    CannotParseMessage,
    WrongEndpointKind,
    EmptyData,
    InvalidData,
    ErrorDateTime,
    MessageParsingError,
    InternalServerError,
    UnknownEndpoint,

    // Common matchmaking
    GameModeNotFound,

    //CreateGame
    ConflictingInputData,
    IncorrectCountValidUsers,
    IncorrectCountTickets,
    IncorrectCountPlayersInTeam,

    // BeginGameSearch
    EmptyUserIds,
    UserIdsDoNotIncludeUser,
    NotAllUsersAreReal,
    SomeUsersHaveActiveGames,
    SomeUsersHaveActiveParties,

    // CancelGameSearch
    UserHasNoActiveParty,

    // ResolveGameStart
    GameNotFound,
    WrongGameStatus,
    WrongGameIdForUser,
    
    // User
    UnknownUser,
}
