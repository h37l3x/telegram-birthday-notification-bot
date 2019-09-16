namespace BirthdayNotificationService.Common.Enums
{
    public enum BirthdayNotificationScheduleBotCommandTypes
    {
        None = 0,
        SubscribeUserToNotification = 1,
        UnsubscribeUserFromNotification = 2,
        SetNotificationMessage = 3,
        SetNotificationDelayInDays = 4,
        ImportBirthdays = 5,
        Birthdays = 6,
        ClearBirthdays = 7
    }
}