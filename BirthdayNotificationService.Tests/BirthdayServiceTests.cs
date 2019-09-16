using System;
using System.Collections.Generic;
using System.Globalization;

using BirthdayNotificationService.Common.Enums;
using BirthdayNotificationService.Domain.Services.BirthdayNotificationServices;
using BirthdayNotificationService.Persistence.Entities;

using NUnit.Framework;

namespace Tests
{
    public class BirthdayServiceTests
    {
        [TestCase("26.08.2019", "25.08.2019", 1)]
        [TestCase("26.08.2019", "26.08.2019", 0)]
        [TestCase("25.08.2019", "26.08.2019", 1)]
        public void DaysDiff_Match_Specified(string todayStr, string birthdayStr, int result)
        {
            var today = DateTime.ParseExact(todayStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            var birthday = DateTime.ParseExact(birthdayStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            var daysDiff = Math.Floor(Math.Abs((birthday - today).TotalDays));
        }

        [TestCase("26.08.2019", 1, 16)]
        [TestCase("27.08.2019", 1, 16)]
        public void GetBirthdaysForNotification_BirthdaysCount_Match_Specified(string todayStr, byte daysCountBeforeNotificaiton, int correctResult)
        {
            var today = DateTime.ParseExact(todayStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            var service = new BirthdayService();

            var result = service.GetBirthdaysForNotification(today, GetBirthdaySchedule(daysCountBeforeNotificaiton));

            Assert.AreEqual(correctResult, result.Count);
        }

        [TestCase("26.08.2019", "25.08.2019", 1, BirthdayDatePeriodTypes.InThePast)]
        [TestCase("26.08.2019", "26.07.2019", 1, BirthdayDatePeriodTypes.InThePast)]
        [TestCase("26.08.2019", "26.08.2019", 1, BirthdayDatePeriodTypes.Today)]
        [TestCase("27.08.2019", "28.08.2019", 1, BirthdayDatePeriodTypes.InFuture)]
        [TestCase("27.08.2019", "27.09.2019", 1, BirthdayDatePeriodTypes.InFuture)]
        [TestCase("27.08.2019", "29.09.2019", 1, BirthdayDatePeriodTypes.InFuture)]
        public void GetBirthdayDatePeriodType_BirthdayDatePeriodType_Match_Specified(string todayStr, string birthdayStr, byte daysCountBeforeNotificaiton, BirthdayDatePeriodTypes correctResult)
        {
            var today = DateTime.ParseExact(todayStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            var birthday = DateTime.ParseExact(birthdayStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            var service = new BirthdayService();

            var result = service.GetBirthdayDatePeriodType(today, birthday);

            Assert.AreEqual(correctResult, result);
        }

        private BirthdaySchedule GetBirthdaySchedule(byte daysCountBeforeNotificaiton)
        {
            var birthdaySchedule = new BirthdaySchedule
            {
                Id = 1,
                TelegramChatId = 1,
                TelegramChat = new TelegramChat
                {
                    Id = 1,
                    ChatExternalId = 1,
                    LastCommandType = BirthdayNotificationScheduleBotCommandTypes.None,
                    UserExternalId = 1,
                    Username = "TEST_USER",
                    BirthdayNotificationSchedules = null
                },
                DaysCountBeforeNotificaiton = daysCountBeforeNotificaiton,
                Birthdays = new List<Birthday>()
            };

            birthdaySchedule.Birthdays.Add(new Birthday { Id = 1, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 1, 2), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 2, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 1, 25), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 3, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 2, 11), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 4, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 2, 16), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 5, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 2, 20), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 6, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 5, 3), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 7, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 7, 11), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 8, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 7, 11), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 9, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 7, 27), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 10, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 7, 28), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 11, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 2), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 12, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 5), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 13, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 9), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 14, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 15), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 15, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 18), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 16, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 8, 26), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 17, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 9, 24), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 18, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 11, 8), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 19, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 11, 11), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 20, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 11, 11), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 21, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 12, 1), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 22, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 12, 18), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });
            birthdaySchedule.Birthdays.Add(new Birthday { Id = 23, BirthdayNotificationScheduleId = 1, DateOfBirth = new DateTime(2019, 12, 24), BirthdayNotificationsHistory = new List<BirthdayNotificationHistory>() });

            return birthdaySchedule;
        }
    }
}