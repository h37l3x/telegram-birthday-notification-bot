using System;
using BirthdayNotificationService.Application.Contracts.Commands;
using BirthdayNotificationService.Application.Handlers.Commands;
using BirthdayNotificationService.Common.ConfigOptions;
using BirthdayNotificationService.Domain.Services.BirthdayNotificationServices;
using Hangfire;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BirthdayNotificationService.Application.WebApi.Controllers
{
    [Route("api/bot/telegram/birthday-schedule")]
    public class BirthdayScheduleTelegramBotController : ControllerBase
    {
        private AuthOptions _authOptions;

        public BirthdayScheduleTelegramBotController(IOptionsMonitor<AuthOptions> authOptionsAccessor)
        {
            _authOptions = authOptionsAccessor.CurrentValue;

            authOptionsAccessor.OnChange(x => _authOptions = x);
        }

        [HttpPost]
        [Route("start-bot-settings-job")]
        public IActionResult StartTelegramSettingsBot([FromBody]StartTelegramBotSettingsCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                throw new UnauthorizedAccessException();

            if (_authOptions.Token != command.Token)
                throw new UnauthorizedAccessException();

            var jobId = BackgroundJob.Enqueue<BirthdayScheduleTelegramBotJobsHandler>(x => x.ProcessUpdates(JobCancellationToken.Null));

            return Ok(jobId);
        }

        [HttpPost]
        [Route("start-birthday-notification-job")]
        public IActionResult StartBirthdayNotificationBot([FromBody]StartBirthdayNotificationCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                throw new UnauthorizedAccessException();

            if (_authOptions.Token != command.Token)
                throw new UnauthorizedAccessException();

            RecurringJob.AddOrUpdate<BirthdayScheduleTelegramBotJobsHandler>(x => x.Notify(JobCancellationToken.Null), Cron.Daily(command.HoursUTC, 0));

            return Ok();
        }
    }
}