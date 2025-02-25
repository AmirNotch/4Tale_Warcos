using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lobby.Migrations
{
    /// <inheritdoc />
    public partial class Users_Game_Maps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "game_modes",
                columns: table => new
                {
                    game_mode_kind = table.Column<string>(type: "text", nullable: false),
                    is_team = table.Column<bool>(type: "boolean", nullable: false),
                    number_of_players = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_modes", x => x.game_mode_kind);
                });

            migrationBuilder.CreateTable(
                name: "maps",
                columns: table => new
                {
                    map_kind = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maps", x => x.map_kind);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    removed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "game_regimes",
                columns: table => new
                {
                    game_regime_kind = table.Column<string>(type: "text", nullable: false),
                    game_mode = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_regimes", x => x.game_regime_kind);
                    table.ForeignKey(
                        name: "FK_game_regimes_game_modes_game_mode",
                        column: x => x.game_mode,
                        principalTable: "game_modes",
                        principalColumn: "game_mode_kind",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_regime_entries",
                columns: table => new
                {
                    game_regime_entry_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    game_mode = table.Column<string>(type: "text", nullable: false),
                    game_regime = table.Column<string>(type: "text", nullable: false),
                    map_kind = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_regime_entries", x => x.game_regime_entry_id);
                    table.ForeignKey(
                        name: "FK_game_regime_entries_game_modes_game_mode",
                        column: x => x.game_mode,
                        principalTable: "game_modes",
                        principalColumn: "game_mode_kind",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_regime_entries_game_regimes_game_regime",
                        column: x => x.game_regime,
                        principalTable: "game_regimes",
                        principalColumn: "game_regime_kind",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_regime_entries_maps_map_kind",
                        column: x => x.map_kind,
                        principalTable: "maps",
                        principalColumn: "map_kind",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    game_server_url = table.Column<string>(type: "text", nullable: false),
                    game_regime_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    game_status = table.Column<string>(type: "text", nullable: false, defaultValue: "CONFIRMATION")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.game_id);
                    table.ForeignKey(
                        name: "FK_games_game_regime_entries_game_regime_entry_id",
                        column: x => x.game_regime_entry_id,
                        principalTable: "game_regime_entries",
                        principalColumn: "game_regime_entry_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "scheduled_game_regimes",
                columns: table => new
                {
                    game_mode = table.Column<string>(type: "text", nullable: false),
                    interval_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    game_regime = table.Column<string>(type: "text", nullable: false),
                    map_kind = table.Column<string>(type: "text", nullable: false),
                    game_regime_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interval_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduled_game_regimes", x => new { x.game_mode, x.interval_start });
                    table.ForeignKey(
                        name: "FK_scheduled_game_regimes_game_modes_game_mode",
                        column: x => x.game_mode,
                        principalTable: "game_modes",
                        principalColumn: "game_mode_kind",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_scheduled_game_regimes_game_regime_entries_game_regime_entr~",
                        column: x => x.game_regime_entry_id,
                        principalTable: "game_regime_entries",
                        principalColumn: "game_regime_entry_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_scheduled_game_regimes_game_regimes_game_regime",
                        column: x => x.game_regime,
                        principalTable: "game_regimes",
                        principalColumn: "game_regime_kind",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_scheduled_game_regimes_maps_map_kind",
                        column: x => x.map_kind,
                        principalTable: "maps",
                        principalColumn: "map_kind",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "game_to_user",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_confirmed_game_start = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    squad = table.Column<int>(type: "integer", nullable: false),
                    kills = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    deaths = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    assists = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    removed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_to_user", x => new { x.game_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_game_to_user_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_game_to_user_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parties",
                columns: table => new
                {
                    party_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    leader_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    removed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    size = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parties", x => x.party_id);
                    table.ForeignKey(
                        name: "FK_parties_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parties_users_leader_user_id",
                        column: x => x.leader_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "party_to_user",
                columns: table => new
                {
                    party_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_confirmed_party_partaking = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    removed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_party_to_user", x => new { x.party_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_party_to_user_parties_party_id",
                        column: x => x.party_id,
                        principalTable: "parties",
                        principalColumn: "party_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_party_to_user_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "game_modes",
                columns: new[] { "game_mode_kind", "created", "is_team", "number_of_players" },
                values: new object[] { "TEAM_FIGHT", new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 18, DateTimeKind.Unspecified).AddTicks(9816), new TimeSpan(0, 0, 0, 0, 0)), true, 12 });

            migrationBuilder.InsertData(
                table: "maps",
                columns: new[] { "map_kind", "created" },
                values: new object[,]
                {
                    { "L_Airport", new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 25, DateTimeKind.Unspecified).AddTicks(2752), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "L_Freeport", new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 25, DateTimeKind.Unspecified).AddTicks(2748), new TimeSpan(0, 0, 0, 0, 0)) },
                    { "L_Hotel", new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 25, DateTimeKind.Unspecified).AddTicks(2754), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "game_regimes",
                columns: new[] { "game_regime_kind", "created", "game_mode" },
                values: new object[] { "CAPTURE_POINT", new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 20, DateTimeKind.Unspecified).AddTicks(9038), new TimeSpan(0, 0, 0, 0, 0)), "TEAM_FIGHT" });

            migrationBuilder.InsertData(
                table: "game_regime_entries",
                columns: new[] { "game_regime_entry_id", "created", "game_mode", "game_regime", "map_kind" },
                values: new object[,]
                {
                    { new Guid("461fb474-c5af-4dec-9fe5-2bf52fcfccc5"), new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 22, DateTimeKind.Unspecified).AddTicks(9660), new TimeSpan(0, 0, 0, 0, 0)), "TEAM_FIGHT", "CAPTURE_POINT", "L_Hotel" },
                    { new Guid("c53427e4-e29b-411a-bbb7-4f8005130878"), new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 22, DateTimeKind.Unspecified).AddTicks(9653), new TimeSpan(0, 0, 0, 0, 0)), "TEAM_FIGHT", "CAPTURE_POINT", "L_Freeport" },
                    { new Guid("ecc0d782-c0c7-4a82-a164-1a878964e57e"), new DateTimeOffset(new DateTime(2024, 10, 16, 12, 15, 55, 22, DateTimeKind.Unspecified).AddTicks(9657), new TimeSpan(0, 0, 0, 0, 0)), "TEAM_FIGHT", "CAPTURE_POINT", "L_Airport" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_regime_entries_game_mode_game_regime_map_kind",
                table: "game_regime_entries",
                columns: new[] { "game_mode", "game_regime", "map_kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_regime_entries_game_regime",
                table: "game_regime_entries",
                column: "game_regime");

            migrationBuilder.CreateIndex(
                name: "IX_game_regime_entries_map_kind",
                table: "game_regime_entries",
                column: "map_kind");

            migrationBuilder.CreateIndex(
                name: "IX_game_regimes_game_mode",
                table: "game_regimes",
                column: "game_mode");

            migrationBuilder.CreateIndex(
                name: "game_to_user_back_order_idx",
                table: "game_to_user",
                columns: new[] { "user_id", "game_id" });

            migrationBuilder.CreateIndex(
                name: "IX_games_game_regime_entry_id",
                table: "games",
                column: "game_regime_entry_id");

            migrationBuilder.CreateIndex(
                name: "IX_parties_game_id",
                table: "parties",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_parties_leader_user_id",
                table: "parties",
                column: "leader_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_party_to_user_user_id_party_id",
                table: "party_to_user",
                columns: new[] { "user_id", "party_id" });

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_game_regimes_game_regime",
                table: "scheduled_game_regimes",
                column: "game_regime");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_game_regimes_game_regime_entry_id",
                table: "scheduled_game_regimes",
                column: "game_regime_entry_id");

            migrationBuilder.CreateIndex(
                name: "IX_scheduled_game_regimes_map_kind",
                table: "scheduled_game_regimes",
                column: "map_kind");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_to_user");

            migrationBuilder.DropTable(
                name: "party_to_user");

            migrationBuilder.DropTable(
                name: "scheduled_game_regimes");

            migrationBuilder.DropTable(
                name: "parties");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "game_regime_entries");

            migrationBuilder.DropTable(
                name: "game_regimes");

            migrationBuilder.DropTable(
                name: "maps");

            migrationBuilder.DropTable(
                name: "game_modes");
        }
    }
}
