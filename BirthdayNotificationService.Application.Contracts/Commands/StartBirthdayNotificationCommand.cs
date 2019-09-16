namespace BirthdayNotificationService.Application.Contracts.Commands
{
    public class StartBirthdayNotificationCommand
    {
        public string Token { get; set; }
        public int HoursUTC { get; set; }
    }
}