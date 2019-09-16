using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BirthdayNotificationService.Persistence.Migrations
{
    public partial class BirthdayNotificationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BirthdayNotificationScheduleUsers_BirthdayNotificationSchedules_BirthdayNotificationScheduleId",
                table: "BirthdayNotificationScheduleUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BirthdayNotificationScheduleUsers",
                table: "BirthdayNotificationScheduleUsers");

            migrationBuilder.RenameTable(
                name: "BirthdayNotificationScheduleUsers",
                newName: "Birthdays");

            migrationBuilder.RenameIndex(
                name: "IX_BirthdayNotificationScheduleUsers_BirthdayNotificationScheduleId",
                table: "Birthdays",
                newName: "IX_Birthdays_BirthdayNotificationScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Birthdays",
                table: "Birthdays",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BirthdayNotificationHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationYear = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    BirthdayId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayNotificationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayNotificationHistory_Birthdays_BirthdayId",
                        column: x => x.BirthdayId,
                        principalTable: "Birthdays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayNotificationHistory_BirthdayId",
                table: "BirthdayNotificationHistory",
                column: "BirthdayId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayNotificationHistory_NotificationYear_BirthdayId",
                table: "BirthdayNotificationHistory",
                columns: new[] { "NotificationYear", "BirthdayId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Birthdays_BirthdayNotificationSchedules_BirthdayNotificationScheduleId",
                table: "Birthdays",
                column: "BirthdayNotificationScheduleId",
                principalTable: "BirthdayNotificationSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Birthdays_BirthdayNotificationSchedules_BirthdayNotificationScheduleId",
                table: "Birthdays");

            migrationBuilder.DropTable(
                name: "BirthdayNotificationHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Birthdays",
                table: "Birthdays");

            migrationBuilder.RenameTable(
                name: "Birthdays",
                newName: "BirthdayNotificationScheduleUsers");

            migrationBuilder.RenameIndex(
                name: "IX_Birthdays_BirthdayNotificationScheduleId",
                table: "BirthdayNotificationScheduleUsers",
                newName: "IX_BirthdayNotificationScheduleUsers_BirthdayNotificationScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BirthdayNotificationScheduleUsers",
                table: "BirthdayNotificationScheduleUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BirthdayNotificationScheduleUsers_BirthdayNotificationSchedules_BirthdayNotificationScheduleId",
                table: "BirthdayNotificationScheduleUsers",
                column: "BirthdayNotificationScheduleId",
                principalTable: "BirthdayNotificationSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
