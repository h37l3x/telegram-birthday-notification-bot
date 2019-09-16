using System;
using System.Collections.Generic;
using System.Linq;

using BirthdayNotificationService.Common.Enums;
using BirthdayNotificationService.Persistence.Entities;

namespace BirthdayNotificationService.Domain.Services.BirthdayNotificationServices
{
    public class BirthdayService
    {
        public double GetDaysDiff(DateTime today, DateTime birthday)
        {
            return Math.Floor(Math.Abs((birthday - today).TotalDays));
        }

        public List<Birthday> GetBirthdaysForNotification(DateTime today, BirthdaySchedule schedule)
        {
            return (schedule?.Birthdays ?? new List<Birthday>())
                .Where(x => ShouldNotify(today, x.DateOfBirth, schedule.DaysCountBeforeNotificaiton, x.BirthdayNotificationsHistory))
                .ToList();
        }

        public bool ShouldNotify(DateTime today, DateTime birthday, int daysCountBeforeNotificaiton, List<BirthdayNotificationHistory> birthdayNotificationsHistory)
        {
            var day = new DateTime(today.Year, birthday.Month, birthday.Day);

            var periodType = GetBirthdayDatePeriodType(today, day);

            if (periodType == BirthdayDatePeriodTypes.InThePast)
            {
                var alreadyNotifiedThisYear = (birthdayNotificationsHistory ?? new List<BirthdayNotificationHistory>()).Any(h => h.NotificationYear == today.Year);
                if (!alreadyNotifiedThisYear)
                    return true;
            }
            else if (periodType == BirthdayDatePeriodTypes.Today)
            {
                return true;
            }
            else if (periodType == BirthdayDatePeriodTypes.InFuture)
            {
                var daysDiff = GetDaysDiff(today, day);
                return daysDiff == daysCountBeforeNotificaiton;
            }

            return false;
        }

        public BirthdayDatePeriodTypes GetBirthdayDatePeriodType(DateTime today, DateTime birthday)
        {
            var day = new DateTime(today.Year, birthday.Month, birthday.Day);
            if (day < today.Date)
                return BirthdayDatePeriodTypes.InThePast;

            if (day == today.Date)
                return BirthdayDatePeriodTypes.Today;

            return BirthdayDatePeriodTypes.InFuture;
        }
    }
}
