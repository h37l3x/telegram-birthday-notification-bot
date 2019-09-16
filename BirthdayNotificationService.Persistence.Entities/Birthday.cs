using System;
using System.Collections.Generic;

namespace BirthdayNotificationService.Persistence.Entities
{
    public class Birthday
    {
        public Birthday()
        {

        }

        public Birthday(string firstname, string lastname, DateTime dateofBirth)
        {
            FirstName = firstname;
            LastName = lastname;
            DateOfBirth = dateofBirth;
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public long BirthdayNotificationScheduleId { get; set; }
        public BirthdaySchedule BirthdayNotificationSchedule { get; set; }
        public List<BirthdayNotificationHistory> BirthdayNotificationsHistory { get; set; }

        public bool IsEqualTo(Birthday x)
        {
            return FirstName == x.FirstName && LastName == x.LastName && DateOfBirth == x.DateOfBirth;
        }

        public void UpdateFrom(Birthday x)
        {
            FirstName = x.FirstName;
            LastName = x.LastName;
            DateOfBirth = x.DateOfBirth;
        }
    }
}