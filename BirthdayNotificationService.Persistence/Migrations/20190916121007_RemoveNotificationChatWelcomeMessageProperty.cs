using Microsoft.EntityFrameworkCore.Migrations;

namespace BirthdayNotificationService.Persistence.Migrations
{
    public partial class RemoveNotificationChatWelcomeMessageProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationChatWelcomeMessage",
                table: "BirthdayNotificationSchedules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationChatWelcomeMessage",
                table: "BirthdayNotificationSchedules",
                nullable: true);
        }
    }
}
