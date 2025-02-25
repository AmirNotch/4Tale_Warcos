using Microsoft.EntityFrameworkCore;
using UserProfile.Models.db;
using UserProfile.Models.DbConfigurations;

namespace UserProfile.Models;

public partial class WarcosUserProfileDbContext : DbContext
{
    private readonly bool _isTrackingEnabled;
    
    public WarcosUserProfileDbContext()
    {
        _isTrackingEnabled = false;
    }

    public WarcosUserProfileDbContext(DbContextOptions<WarcosUserProfileDbContext> options, bool isTrackingEnabled)
        : base(options)
    {
        _isTrackingEnabled = isTrackingEnabled;
        ChangeTracker.AutoDetectChangesEnabled = _isTrackingEnabled; // Установка отслеживания
    }
    
    public virtual DbSet<db.UserProfile> UserProfiles { get; set; }
    public virtual DbSet<UserProfileStatsHistory> UserProfileStatsHistories { get; set; }
    public virtual DbSet<Level> Levels { get; set; }
    public virtual DbSet<LevelReward> LevelRewards { get; set; }
    public virtual DbSet<db.Achievement> Achievements { get; set; }
    public virtual DbSet<UserAchievement> UserAchievements { get; set; }
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<UserLevelReward> UserLevelRewards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.

            string pgConnectionEnv = Environment.GetEnvironmentVariable("PG_CONNECTION_USER_PROFILE") ??
                throw new ApplicationException("Environment variable PG_CONNECTION_USER_PROFILE is not set!");
            optionsBuilder.UseNpgsql(pgConnectionEnv!);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarcosUserProfileDbContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}