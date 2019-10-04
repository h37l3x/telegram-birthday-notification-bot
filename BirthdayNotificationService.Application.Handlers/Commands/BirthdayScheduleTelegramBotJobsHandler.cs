using System;
using System.Linq;
using System.Threading.Tasks;

using BirthdayNotificationService.Common.ConfigOptions;
using BirthdayNotificationService.Domain.Services.BirthdayNotificationServices;
using BirthdayNotificationService.Persistence.Repositories;

using ElmahCore;

using Hangfire;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace BirthdayNotificationService.Application.Handlers.Commands
{

    public class BirthdayScheduleTelegramBotJobsHandler
    {
        private AuthOptions _authOptions;
        private readonly ErrorLog _errorLog;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly BirthdayNotificationScheduleRepository _birthdayScheduleTelegramBotRepository;
        private readonly BirthdayNotificationsService _birthdayNotificationsService;

        public BirthdayScheduleTelegramBotJobsHandler(ErrorLog errorLog, 
            IOptionsMonitor<AuthOptions> authOptionsAccessor,
            BirthdayNotificationScheduleRepository birthdayScheduleTelegramBotRepository,
            BirthdayNotificationsService birthdayNotificationsService,
            ITelegramBotClient telegramBotClient)
        {
            _authOptions = authOptionsAccessor.CurrentValue;
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
            _birthdayScheduleTelegramBotRepository = birthdayScheduleTelegramBotRepository ?? throw new ArgumentNullException(nameof(birthdayScheduleTelegramBotRepository));
            _birthdayNotificationsService = birthdayNotificationsService ?? throw new ArgumentNullException(nameof(birthdayNotificationsService));
            _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));

            authOptionsAccessor.OnChange(x => _authOptions = x);
        }

        public async Task ProcessUpdates(IJobCancellationToken cancellationToken)
        {
            try
            {
                var offset = 0;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    offset = await Tick(_telegramBotClient, offset);
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                await _errorLog.LogAsync(new Error(ex));
                throw;
            }
        }

        public async Task Notify(IJobCancellationToken cancellationToken)
        {
            try
            {
                await _birthdayNotificationsService.CheckAndNotify(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (Exception ex)
            {
                await _errorLog.LogAsync(new Error(ex));
                throw;
            }
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