using System;
using System.Threading.Tasks;

using BirthdayNotificationService.Common.Enums;
using BirthdayNotificationService.Persistence.Entities;
using BirthdayNotificationService.Persistence.Repositories;

using Telegram.Bot;

namespace BirthdayNotificationService.Domain.Services.BirthdayNotificationServices
{
    public class BirthdayNotificationsService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly BirthdayService _birthdayService;
        private readonly BirthdayNotificationScheduleRepository _birthdayScheduleTelegramBotRepository;

        public BirthdayNotificationsService(BirthdayNotificationScheduleRepository birthdayScheduleTelegramBotRepository,
            BirthdayService birthdayService,
            ITelegramBotClient botClient)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _birthdayService = birthdayService ?? throw new ArgumentNullException(nameof(birthdayService));
            _birthdayScheduleTelegramBotRepository = birthdayScheduleTelegramBotRepository ?? throw new ArgumentNullException(nameof(birthdayScheduleTelegramBotRepository));
        }

        public async Task CheckAndNotify(Action checkShouldStop)
        {
            var today = DateTime.Now.Date;

            var chats = await _birthdayScheduleTelegramBotRepository.Get();
            foreach (var chat in chats)
            {
                foreach (var schedule in chat.BirthdayNotificationSchedules)
                {
                    var birthdays = _birthdayService.GetBirthdaysForNotification(today, schedule);

                    foreach (var birthday in birthdays)
                    {
                        checkShouldStop();

                        var periodType = _birthdayService.GetBirthdayDatePeriodType(today, birthday.DateOfBirth);

                        var daysDiff = _birthdayService.GetDaysDiff(today, birthday.DateOfBirth);
                        var text = default(string);

                        if (periodType == BirthdayDatePeriodTypes.Today)
                        {
                            text = $"Сегодня день рождения у {birthday.FirstName} {birthday.LastName} {birthday.DateOfBirth.ToString("dd.MM")}";
                        }
                        else if (periodType == BirthdayDatePeriodTypes.InFuture)
                        {
                            text = $"Через {daysDiff} дней день рождения у {birthday.FirstName} {birthday.LastName} {birthday.DateOfBirth.ToString("dd.MM")}";
                        }
                        else if (periodType == BirthdayDatePeriodTypes.InThePast)
                        {
                            text = $"{Math.Abs(daysDiff)} дней назад был день рождения у {birthday.FirstName} {birthday.LastName} {birthday.DateOfBirth.ToString("dd.MM")}";
                        }

                        await _botClient.SendTextMessageAsync(chatId: chat.ChatExternalId, text: text);

                        await _birthdayScheduleTelegramBotRepository.AddNotificationHistory(new BirthdayNotificationHistory
                        {
                            BirthdayId = birthday.Id,
                            CreationDate = today,
                            NotificationYear = today.Year
                        });
                    }
                }
            }
        }
    }
}