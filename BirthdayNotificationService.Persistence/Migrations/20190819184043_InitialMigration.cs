using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BirthdayNotificationService.Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelegramChats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChatExternalId = table.Column<long>(nullable: false),
                    UserExternalId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    LastCommandType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramChats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayNotificationSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationChatWelcomeMessage = table.Column<string>(nullable: true),
                    DaysCountBeforeNotificaiton = table.Column<byte>(nullable: false),
                    TelegramChatId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayNotificationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayNotificationSchedules_TelegramChats_TelegramChatId",
                        column: x => x.TelegramChatId,
                        principalTable: "TelegramChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BirthdayNotificationScheduleUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    BirthdayNotificationScheduleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayNotificationScheduleUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayNotificationScheduleUsers_BirthdayNotificationSchedules_BirthdayNotificationScheduleId",
                        column: x => x.BirthdayNotificationScheduleId,
                        principalTable: "BirthdayNotificationSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayNotificationSchedules_TelegramChatId",
                table: "BirthdayNotificationSchedules",
                column: "TelegramChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayNotificationScheduleUsers_BirthdayNotificationScheduleId",
                table: "BirthdayNotificationScheduleUsers",
                column: "BirthdayNotificationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramChats_ChatExternalId",
                table: "TelegramChats",
                column: "ChatExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramChats_Username",
                table: "TelegramChats",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BirthdayNotificationScheduleUsers");

            migrationBuilder.DropTable(
                name: "BirthdayNotificationSchedules");

            migrationBuilder.DropTable(
                name: "TelegramChats");
        }
    }
}
