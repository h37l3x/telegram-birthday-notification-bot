using System.Collections.Generic;

namespace BirthdayNotificationService.Persistence.Entities
{
    public class BirthdaySchedule
    {
        public long Id { get; set; }

        public byte DaysCountBeforeNotificaiton { get; set; }

        public long TelegramChatId { get; set; }
        public TelegramChat TelegramChat { get; set; }

        public List<Birthday> Birthdays { get; set; }
    }
}