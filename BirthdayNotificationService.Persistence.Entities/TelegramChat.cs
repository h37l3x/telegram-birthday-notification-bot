using System.Collections.Generic;
using BirthdayNotificationService.Common.Enums;

namespace BirthdayNotificationService.Persistence.Entities
{
    public class TelegramChat
    {
        public long Id { get; set; }
        public long ChatExternalId { get; set; }
        public long UserExternalId { get; set; }
        public string Username { get; set; }
        public BirthdayNotificationScheduleBotCommandTypes LastCommandType { get; set; }

        public List<BirthdaySchedule> BirthdayNotificationSchedules { get; set; }
    }
}