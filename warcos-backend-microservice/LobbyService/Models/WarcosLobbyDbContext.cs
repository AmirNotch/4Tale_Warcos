using Lobby.Models.db;
using Microsoft.EntityFrameworkCore;

namespace Lobby.Models;

public partial class WarcosLobbyDbContext : DbContext
{
    private readonly bool _isTrackingEnabled;
    
    public WarcosLobbyDbContext()
    {
        _isTrackingEnabled = false;
    }

    public WarcosLobbyDbContext(DbContextOptions<WarcosLobbyDbContext> options, bool isTrackingEnabled)
        : base(options)
    {
        _isTrackingEnabled = isTrackingEnabled;
        ChangeTracker.AutoDetectChangesEnabled = _isTrackingEnabled; // Установка отслеживания
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameMode> GameModes { get; set; }

    public virtual DbSet<GameRegime> GameRegimes { get; set; }

    public virtual DbSet<GameRegimeEntry> GameRegimeEntries { get; set; }

    public virtual DbSet<GameToUser> GameToUsers { get; set; }

    public virtual DbSet<Map> Maps { get; set; }

    public virtual DbSet<Party> Parties { get; set; }

    public virtual DbSet<PartyToUser> PartyToUsers { get; set; }

    public virtual DbSet<ScheduledGameRegime> ScheduledGameRegimes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

            string pgConnectionEnv = Environment.GetEnvironmentVariable("PG_CONNECTION") ??
                throw new ApplicationException("Environment variable PG_CONNECTION is not set!");
            optionsBuilder.UseNpgsql(pgConnectionEnv!);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarcosLobbyDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
