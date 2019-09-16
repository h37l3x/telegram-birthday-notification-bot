using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BirthdayNotificationService.Common.ConfigOptions;
using BirthdayNotificationService.Domain.Services.BirthdayNotificationServices;
using BirthdayNotificationService.Persistence.Repositories;

using Hangfire;

using Microsoft.Extensions.Options;

using MihaZupan;
using Telegram.Bot;

namespace BirthdayNotificationService.Application.Handlers.Commands
{

    public class BirthdayScheduleTelegramBotJobsHandler
    {
        private AuthOptions _authOptions;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly BirthdayNotificationScheduleRepository _birthdayScheduleTelegramBotRepository;
        private readonly BirthdayNotificationsService _birthdayNotificationsService;

        public BirthdayScheduleTelegramBotJobsHandler(IOptionsMonitor<AuthOptions> authOptionsAccessor,
            BirthdayNotificationScheduleRepository birthdayScheduleTelegramBotRepository,
            BirthdayNotificationsService birthdayNotificationsService,
            ITelegramBotClient telegramBotClient)
        {
            _authOptions = authOptionsAccessor.CurrentValue;
            _birthdayScheduleTelegramBotRepository = birthdayScheduleTelegramBotRepository ?? throw new ArgumentNullException(nameof(birthdayScheduleTelegramBotRepository));
            _birthdayNotificationsService = birthdayNotificationsService ?? throw new ArgumentNullException(nameof(birthdayNotificationsService));
            _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));

            authOptionsAccessor.OnChange(x => _authOptions = x);
        }

        public async Task ProcessUpdates(IJobCancellationToken cancellationToken)
        {
            var offset = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                offset = await Tick(_telegramBotClient, offset);
                Thread.Sleep(1000);
            }
        }

        public async Task Notify(IJobCancellationToken cancellationToken)
        {
            await _birthdayNotificationsService.CheckAndNotify(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
            });
        }

        private async Task<int> Tick(ITelegramBotClient botClient, int offset)
        {
            var updates = await botClient.GetUpdatesAsync(offset);
            if (updates == null && !updates.Any())
                return offset;

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                var service = new BirthdayNotificationBotService(_birthdayScheduleTelegramBotRepository, botClient);
                await service.ProccessMessage(update);
            }

            return offset;
        }
    }
}