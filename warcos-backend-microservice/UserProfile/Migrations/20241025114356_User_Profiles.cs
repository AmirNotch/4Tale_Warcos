using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserProfile.Migrations
{
    /// <inheritdoc />
    public partial class User_Profiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    item_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    level = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    experience_points = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_levels", x => x.level);
                });

            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    requirement_type = table.Column<int>(type: "integer", nullable: false),
                    requirement_value = table.Column<int>(type: "integer", nullable: false),
                    reward_type = table.Column<int>(type: "integer", nullable: false),
                    reward_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.achievement_id);
                    table.ForeignKey(
                        name: "FK_achievements_items_reward_item_id",
                        column: x => x.reward_item_id,
                        principalTable: "items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "level_rewards",
                columns: table => new
                {
                    level_id = table.Column<int>(type: "integer", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reward_type = table.Column<int>(type: "integer", nullable: false),
                    reward_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_level_rewards", x => new { x.level_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_level_rewards_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_level_rewards_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    level_id = table.Column<int>(type: "integer", nullable: false),
                    experience_points = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    kills = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    deaths = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    assists = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    wins = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    losses = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    matches_played = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    play_time = table.Column<TimeSpan>(type: "interval", nullable: false, defaultValueSql: "INTERVAL '0'"),
                    headshot_kills = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    melee_kills = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    kill_streak = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_profiles_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achieved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => new { x.user_id, x.achievement_id });
                    table.ForeignKey(
                        name: "FK_user_achievements_achievements_achievement_id",
                        column: x => x.achievement_id,
                        principalTable: "achievements",
                        principalColumn: "achievement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_achievements_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_achievements_reward_item_id",
                table: "achievements",
                column: "reward_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_level_rewards_item_id",
                table: "level_rewards",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_level_rewards_level_id",
                table: "level_rewards",
                column: "level_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_achievement_id",
                table: "user_achievements",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_level_id",
                table: "user_profiles",
                column: "level_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "level_rewards");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "levels");
        }
    }
}
