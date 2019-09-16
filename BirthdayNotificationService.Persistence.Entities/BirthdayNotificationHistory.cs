using System;
using System.Collections.Generic;
using System.Text;

namespace BirthdayNotificationService.Persistence.Entities
{
    public class BirthdayNotificationHistory
    {
        public long Id { get; set; }
        public int NotificationYear { get; set; }
        public DateTime CreationDate { get; set; }

        public long BirthdayId { get; set; }
        public Birthday Birthday { get; set; }
    }
}