using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserProfile.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserProfileTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profile_stats_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level_id = table.Column<int>(type: "integer", nullable: false),
                    experience_points = table.Column<int>(type: "integer", nullable: false),
                    kills = table.Column<int>(type: "integer", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    losses = table.Column<int>(type: "integer", nullable: false),
                    matches_played = table.Column<int>(type: "integer", nullable: false),
                    play_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    headshot_kills = table.Column<int>(type: "integer", nullable: false),
                    melee_kills = table.Column<int>(type: "integer", nullable: false),
                    kill_streak = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profile_stats_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_profile_stats_history_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_profile_stats_history_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "items",
                columns: new[] { "item_id", "CreatedAt", "name" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426614174000"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8685), "combat_helmet" },
                    { new Guid("4567d890-1234-56d7-e890-123456789012"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8692), "armor_piercing_ammo" },
                    { new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8710), "urban_camo_skin" },
                    { new Guid("987f6543-a21b-34d2-c321-765432109876"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8689), "high_precision_scope" },
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8695), "advanced_grenade" },
                    { new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8720), "night_ops_skin" },
                    { new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8723), "golden_dragon_skin" },
                    { new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8713), "desert_storm_skin" },
                    { new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8717), "jungle_warfare_skin" },
                    { new Guid("fedc4321-ba98-76d5-c432-109876543210"), new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8698), "reinforced_armor" }
                });

            migrationBuilder.InsertData(
                table: "levels",
                columns: new[] { "level", "experience_points" },
                values: new object[,]
                {
                    { 1, 1000 },
                    { 2, 2000 },
                    { 3, 4000 },
                    { 4, 7000 },
                    { 5, 10000 },
                    { 6, 13000 },
                    { 7, 18000 },
                    { 8, 23000 },
                    { 9, 29000 },
                    { 10, 35000 }
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "achievement_id", "name", "requirement_type", "requirement_value", "reward_item_id", "reward_type" },
                values: new object[,]
                {
                    { new Guid("11292e61-3cbb-49b7-9910-4d11e1ee33dc"), "Master Tactician", 2, 50, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 2 },
                    { new Guid("28825d5e-9061-47d3-9bf1-7737d030adb3"), "Sharp Shooter", 0, 10, new Guid("987f6543-a21b-34d2-c321-765432109876"), 2 },
                    { new Guid("94577782-978b-4829-be0e-456215ccf47d"), "Unstoppable", 4, 5, new Guid("4567d890-1234-56d7-e890-123456789012"), 2 },
                    { new Guid("e3cf5e22-d6c6-49fb-850e-ae16306deb19"), "First Blood", 3, 1, new Guid("123e4567-e89b-12d3-a456-426614174000"), 2 },
                    { new Guid("f4fb5488-8f03-4e95-9fea-76640973d33c"), "Marathon Runner", 5, 1440, new Guid("fedc4321-ba98-76d5-c432-109876543210"), 2 }
                });

            migrationBuilder.InsertData(
                table: "level_rewards",
                columns: new[] { "item_id", "level_id", "reward_name", "reward_type" },
                values: new object[,]
                {
                    { new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"), 1, "Urban Camo Skin", 0 },
                    { new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"), 2, "Desert Storm Skin", 0 },
                    { new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"), 3, "Jungle Warfare Skin", 0 },
                    { new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"), 4, "Night Ops Skin", 0 },
                    { new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"), 5, "Golden Dragon Skin", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_stats_history_level_id",
                table: "user_profile_stats_history",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profile_stats_history_user_id",
                table: "user_profile_stats_history",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_profile_stats_history");

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("11292e61-3cbb-49b7-9910-4d11e1ee33dc"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("28825d5e-9061-47d3-9bf1-7737d030adb3"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("94577782-978b-4829-be0e-456215ccf47d"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("e3cf5e22-d6c6-49fb-850e-ae16306deb19"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("f4fb5488-8f03-4e95-9fea-76640973d33c"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumns: new[] { "item_id", "level_id" },
                keyValues: new object[] { new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"), 1 });

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumns: new[] { "item_id", "level_id" },
                keyValues: new object[] { new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"), 2 });

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumns: new[] { "item_id", "level_id" },
                keyValues: new object[] { new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"), 3 });

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumns: new[] { "item_id", "level_id" },
                keyValues: new object[] { new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"), 4 });

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumns: new[] { "item_id", "level_id" },
                keyValues: new object[] { new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"), 5 });

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174000"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("4567d890-1234-56d7-e890-123456789012"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("987f6543-a21b-34d2-c321-765432109876"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"));

            migrationBuilder.DeleteData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("fedc4321-ba98-76d5-c432-109876543210"));

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "level",
                keyValue: 5);
        }
    }
}
