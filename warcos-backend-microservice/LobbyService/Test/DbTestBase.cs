using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Lobby.Models;
using Lobby.Models.db;

namespace Lobby.Test;

public abstract class DbTestBase : IDisposable {
    protected static readonly string gameModeBattleRoyale = "BATTLE_ROYALE";
    protected static readonly string gameModeTeamFight = "TEAM_FIGHT";
    protected static readonly string gameRegimeBattleRoyale = "BATTLE_ROYALE";
    protected static readonly string gameRegimeDeathmatch = "TEAM_DEATHMATCH";
    protected static readonly string gameRegimePayload = "TEAM_PAYLOAD";
    protected static readonly string gameRegimeCapturePoint = "CAPTURE_POINT";
    protected static readonly string mapFreeport = "FREEPORT";
    protected static readonly string mapIsland = "ISLAND";
    protected static readonly string mapOasis = "OASIS";
    protected static readonly Guid gameRegimeIdBattleRoyale = Guid.NewGuid();
    protected static readonly Guid gameRegimeIdDeathmatchFreeport = Guid.NewGuid();
    protected static readonly Guid gameRegimeIdDeathmatchIsland = Guid.NewGuid();
    protected static readonly Guid gameRegimeIdCapturePointFreeport = Guid.NewGuid();
    protected static readonly Guid gameRegimeIdCapturePointIsland = Guid.NewGuid();
    protected static readonly Guid gameRegimeIdPayloadIsland = Guid.NewGuid();

    protected SqliteConnection Connection { get; private set; }
    protected WarcosLobbyDbContext DbContext { get; private set; }

    public DbTestBase() {
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var contextOptions = new DbContextOptionsBuilder<WarcosLobbyDbContext>()
            .UseSqlite(Connection)
            .EnableSensitiveDataLogging()
            .Options;

        // Create the schema without data
        DbContext = new WarcosLobbyDbContext(contextOptions, false);
        if (!DbContext.Database.EnsureCreated()) {
            throw new ApplicationException("Couldn't initialize test db");
        }

        initializeDB();
    }

    public void initializeDB() {
        DbContext.GameModes.Add(new GameMode { GameModeKind = gameModeBattleRoyale, NumberOfPlayers = 30, IsTeam = false });
        DbContext.GameModes.Add(new GameMode { GameModeKind = gameModeTeamFight, NumberOfPlayers = 12, IsTeam = true });
        DbContext.GameRegimes.Add(new GameRegime { GameMode = gameModeBattleRoyale, GameRegimeKind = gameRegimeBattleRoyale });
        DbContext.GameRegimes.Add(new GameRegime { GameMode = gameModeTeamFight, GameRegimeKind = gameRegimeDeathmatch });
        DbContext.GameRegimes.Add(new GameRegime { GameMode = gameModeTeamFight, GameRegimeKind = gameRegimeCapturePoint });
        DbContext.GameRegimes.Add(new GameRegime { GameMode = gameModeTeamFight, GameRegimeKind = gameRegimePayload });
        DbContext.Maps.Add(new Map { MapKind = mapOasis });
        DbContext.Maps.Add(new Map { MapKind = mapFreeport });
        DbContext.Maps.Add(new Map { MapKind = mapIsland });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdBattleRoyale,
                GameMode = gameModeBattleRoyale, GameRegime = gameRegimeBattleRoyale, MapKind = mapOasis
        });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdDeathmatchIsland,
                GameMode = gameModeTeamFight, GameRegime = gameRegimeDeathmatch, MapKind = mapIsland
        });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdDeathmatchFreeport,
                GameMode = gameModeTeamFight, GameRegime = gameRegimeDeathmatch, MapKind = mapFreeport
        });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdCapturePointIsland,
                GameMode = gameModeTeamFight, GameRegime = gameRegimeCapturePoint, MapKind = mapIsland
        });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdCapturePointFreeport,
                GameMode = gameModeTeamFight, GameRegime = gameRegimeCapturePoint, MapKind = mapFreeport
        });
        DbContext.GameRegimeEntries.Add(new GameRegimeEntry { GameRegimeEntryId = gameRegimeIdPayloadIsland,
                GameMode = gameModeTeamFight, GameRegime = gameRegimePayload, MapKind = mapIsland
        });
    }

    public void Dispose() {
        Connection.Dispose();
    }
}
