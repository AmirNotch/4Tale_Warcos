using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserProfile.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserLevelRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_level_rewards_levels_level_id",
                table: "level_rewards");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_achievements_achievement_id",
                table: "user_achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_user_profiles_user_id",
                table: "user_achievements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_level_rewards",
                table: "level_rewards");

            migrationBuilder.DropIndex(
                name: "IX_level_rewards_level_id",
                table: "level_rewards");

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

            migrationBuilder.AddColumn<Guid>(
                name: "RewardId",
                table: "level_rewards",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "LevelId1",
                table: "level_rewards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_level_rewards",
                table: "level_rewards",
                column: "RewardId");

            migrationBuilder.CreateTable(
                name: "users_level_rewards",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reward_id = table.Column<Guid>(type: "uuid", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_level_rewards", x => new { x.user_id, x.reward_id });
                    table.ForeignKey(
                        name: "FK_users_level_rewards_level_rewards_reward_id",
                        column: x => x.reward_id,
                        principalTable: "level_rewards",
                        principalColumn: "RewardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_level_rewards_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "achievement_id", "name", "requirement_type", "requirement_value", "reward_item_id", "reward_type" },
                values: new object[,]
                {
                    { new Guid("76a5189f-521d-401b-94ae-76f1b4a81ed3"), "Master Tactician", 2, 50, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), 2 },
                    { new Guid("7d251b3b-b781-44ec-89bf-32bf02cf1380"), "Sharp Shooter", 0, 10, new Guid("987f6543-a21b-34d2-c321-765432109876"), 2 },
                    { new Guid("8598de27-d6ce-434e-84b0-1613926ba9c9"), "First Blood", 3, 1, new Guid("123e4567-e89b-12d3-a456-426614174000"), 2 },
                    { new Guid("9acbb8ee-931c-4e3c-b289-750d40be0c32"), "Unstoppable", 4, 5, new Guid("4567d890-1234-56d7-e890-123456789012"), 2 },
                    { new Guid("b849e354-9a68-4e00-8a63-d19f15fa8cbc"), "Marathon Runner", 5, 1440, new Guid("fedc4321-ba98-76d5-c432-109876543210"), 2 }
                });

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174000"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("4567d890-1234-56d7-e890-123456789012"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("987f6543-a21b-34d2-c321-765432109876"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("fedc4321-ba98-76d5-c432-109876543210"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 5, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "level_rewards",
                columns: new[] { "RewardId", "item_id", "level_id", "LevelId1", "reward_name", "reward_type" },
                values: new object[,]
                {
                    { new Guid("1d5f9306-2970-4d8b-a2a8-6cb9b92ed737"), new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"), 2, null, "Desert Storm Skin", 0 },
                    { new Guid("2e77e8b0-53a4-4c56-893e-f367b8de4b84"), new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"), 1, null, "Urban Camo Skin", 0 },
                    { new Guid("46162b0b-68cd-43d3-ada4-f800d2f0aac6"), new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"), 3, null, "Jungle Warfare Skin", 0 },
                    { new Guid("838cb5a4-0c27-4152-a40e-e21e4e5c0682"), new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"), 5, null, "Golden Dragon Skin", 0 },
                    { new Guid("f55e27c4-2321-47ad-9fdc-0d84692e40d1"), new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"), 4, null, "Night Ops Skin", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_level_rewards_level_id",
                table: "level_rewards",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "IX_level_rewards_LevelId1",
                table: "level_rewards",
                column: "LevelId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_level_rewards_reward_id",
                table: "users_level_rewards",
                column: "reward_id");

            migrationBuilder.AddForeignKey(
                name: "FK_level_rewards_levels_LevelId1",
                table: "level_rewards",
                column: "LevelId1",
                principalTable: "levels",
                principalColumn: "level");

            migrationBuilder.AddForeignKey(
                name: "FK_level_rewards_levels_level_id",
                table: "level_rewards",
                column: "level_id",
                principalTable: "levels",
                principalColumn: "level",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_achievements_achievement_id",
                table: "user_achievements",
                column: "achievement_id",
                principalTable: "achievements",
                principalColumn: "achievement_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_user_profiles_user_id",
                table: "user_achievements",
                column: "user_id",
                principalTable: "user_profiles",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_level_rewards_levels_LevelId1",
                table: "level_rewards");

            migrationBuilder.DropForeignKey(
                name: "FK_level_rewards_levels_level_id",
                table: "level_rewards");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_achievements_achievement_id",
                table: "user_achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_user_profiles_user_id",
                table: "user_achievements");

            migrationBuilder.DropTable(
                name: "users_level_rewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_level_rewards",
                table: "level_rewards");

            migrationBuilder.DropIndex(
                name: "IX_level_rewards_level_id",
                table: "level_rewards");

            migrationBuilder.DropIndex(
                name: "IX_level_rewards_LevelId1",
                table: "level_rewards");

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("76a5189f-521d-401b-94ae-76f1b4a81ed3"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("7d251b3b-b781-44ec-89bf-32bf02cf1380"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("8598de27-d6ce-434e-84b0-1613926ba9c9"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("9acbb8ee-931c-4e3c-b289-750d40be0c32"));

            migrationBuilder.DeleteData(
                table: "achievements",
                keyColumn: "achievement_id",
                keyValue: new Guid("b849e354-9a68-4e00-8a63-d19f15fa8cbc"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumn: "RewardId",
                keyColumnType: "uuid",
                keyValue: new Guid("1d5f9306-2970-4d8b-a2a8-6cb9b92ed737"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumn: "RewardId",
                keyColumnType: "uuid",
                keyValue: new Guid("2e77e8b0-53a4-4c56-893e-f367b8de4b84"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumn: "RewardId",
                keyColumnType: "uuid",
                keyValue: new Guid("46162b0b-68cd-43d3-ada4-f800d2f0aac6"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumn: "RewardId",
                keyColumnType: "uuid",
                keyValue: new Guid("838cb5a4-0c27-4152-a40e-e21e4e5c0682"));

            migrationBuilder.DeleteData(
                table: "level_rewards",
                keyColumn: "RewardId",
                keyColumnType: "uuid",
                keyValue: new Guid("f55e27c4-2321-47ad-9fdc-0d84692e40d1"));

            migrationBuilder.DropColumn(
                name: "RewardId",
                table: "level_rewards");

            migrationBuilder.DropColumn(
                name: "LevelId1",
                table: "level_rewards");

            migrationBuilder.AddPrimaryKey(
                name: "PK_level_rewards",
                table: "level_rewards",
                columns: new[] { "level_id", "item_id" });

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

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174000"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8685));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("4567d890-1234-56d7-e890-123456789012"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8692));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("650edfbf-8e64-4381-ba48-6aa5bc4b978d"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8710));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("987f6543-a21b-34d2-c321-765432109876"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8689));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8695));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("b5877b11-5977-4168-b9bf-c4f1d81ae689"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("cae5c7ba-8b41-44ba-86c6-ac13731f8acc"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8723));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("dade044f-09b6-4e60-b526-62d18844fd2a"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8713));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("e941dbdf-9d4a-4130-8855-19432e42dcad"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8717));

            migrationBuilder.UpdateData(
                table: "items",
                keyColumn: "item_id",
                keyValue: new Guid("fedc4321-ba98-76d5-c432-109876543210"),
                column: "CreatedAt",
                value: new DateTime(2024, 10, 28, 10, 41, 43, 97, DateTimeKind.Utc).AddTicks(8698));

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
                name: "IX_level_rewards_level_id",
                table: "level_rewards",
                column: "level_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_level_rewards_levels_level_id",
                table: "level_rewards",
                column: "level_id",
                principalTable: "levels",
                principalColumn: "level",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_achievements_achievement_id",
                table: "user_achievements",
                column: "achievement_id",
                principalTable: "achievements",
                principalColumn: "achievement_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_user_profiles_user_id",
                table: "user_achievements",
                column: "user_id",
                principalTable: "user_profiles",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
