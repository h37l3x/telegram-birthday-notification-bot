using System;

namespace BirthdayNotificationService.Common.Enums
{
    public enum BirthdayNotificationScheduleBotCommandTypes
    {
        None = 0,
        SetNotificationDelayInDays = 4,
        ImportBirthdays = 5,
        Birthdays = 6,
        ClearBirthdays = 7
    }
}