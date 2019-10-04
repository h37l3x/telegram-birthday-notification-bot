using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BirthdayNotificationService.Common.Enums;
using BirthdayNotificationService.Common.Extensions;
using BirthdayNotificationService.Domain.Contracts.Services;
using BirthdayNotificationService.Persistence.Entities;
using BirthdayNotificationService.Persistence.Repositories;

using Newtonsoft.Json;

using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BirthdayNotificationService.Domain.Services.BirthdayNotificationServices
{
    public class BirthdayNotificationBotService : IBirthdayNotificationService
    {
        private Dictionary<string, BirthdayNotificationScheduleBotCommandTypes> _commandNameToTypeMap = new Dictionary<string, BirthdayNotificationScheduleBotCommandTypes>
        {
            { "/set_notification_delay_in_days", BirthdayNotificationScheduleBotCommandTypes.SetNotificationDelayInDays },
            { "/import_birthdays", BirthdayNotificationScheduleBotCommandTypes.ImportBirthdays },
            { "/birthdays", BirthdayNotificationScheduleBotCommandTypes.Birthdays },
            { "/clear_birthdays", BirthdayNotificationScheduleBotCommandTypes.ClearBirthdays },
        };

        protected readonly ITelegramBotClient BotClient;
        protected readonly BirthdayNotificationScheduleRepository BirthdayScheduleTelegramBotRepository;

        public BirthdayNotificationBotService(BirthdayNotificationScheduleRepository birthdayScheduleTelegramBotRepository, ITelegramBotClient botClient)
        {
            BotClient = botClient;
            BirthdayScheduleTelegramBotRepository = birthdayScheduleTelegramBotRepository;
        }

        /// <summary>
        /// Обработка обновления
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task ProccessMessage(Update update)
        {
            if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message || (update.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text && update.Message.Type != Telegram.Bot.Types.Enums.MessageType.Contact))
            {
                await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Для меня данный тип сообщения неизвестен", replyToMessageId: update.Message.MessageId);
                return;
            }

            if (update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && string.IsNullOrWhiteSpace(update.Message.Text))
            {
                await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Я не понимаю пустые сообщения", replyToMessageId: update.Message.MessageId);
                return;
            }

            if (update.Message.From.IsBot)
                return;

            var telegramChat = await BirthdayScheduleTelegramBotRepository.GetChat(update.Message.Chat.Id);
            if (telegramChat == null)
            {
                telegramChat = await BirthdayScheduleTelegramBotRepository.AddChat(update.Message.Chat.Id, update.Message.From.Id, update.Message.From.Username);
                telegramChat = await BirthdayScheduleTelegramBotRepository.GetChat(update.Message.Chat.Id);
            }

            var commandType = GetCommandType(update);
            if (commandType == null)
            {
                if (telegramChat.LastCommandType == BirthdayNotificationScheduleBotCommandTypes.None)
                {
                    await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Бот запутался. Пожалуйста выберите команду", replyToMessageId: update.Message.MessageId);
                    return;
                }

                switch (telegramChat.LastCommandType)
                {
                    case BirthdayNotificationScheduleBotCommandTypes.ImportBirthdays:
                        try
                        {
                            if (telegramChat.BirthdayNotificationSchedules == null)
                                telegramChat.BirthdayNotificationSchedules = new List<BirthdaySchedule>();

                            if (!telegramChat.BirthdayNotificationSchedules.Any())
                                telegramChat.BirthdayNotificationSchedules.Add(new BirthdaySchedule
                                {
                                    DaysCountBeforeNotificaiton = 2,
                                    TelegramChatId = telegramChat.Id
                                });

                            var schedule = telegramChat.BirthdayNotificationSchedules.First();
                            schedule.Birthdays = new List<Birthday>();

                            var lines = update.Message.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length != 3)
                                    continue;

                                var firstname = parts[0].Trim();
                                var lastname = parts[1].Trim();
                                var birthdayStr = parts[2].Trim();

                                if (!DateTime.TryParseExact(birthdayStr, "dd.MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateofBirth))
                                    continue;

                                var birthday = new Birthday(firstname, lastname, dateofBirth);
                                var existing = schedule.Birthdays.FirstOrDefault(x => x.IsEqualTo(birthday));
                                if (existing == null)
                                {
                                    schedule.Birthdays.Add(birthday);
                                }
                                else
                                {
                                    existing.UpdateFrom(birthday);
                                }
                            }

                            var text = $@"Данные импортированы.{Environment.NewLine}Уведомление будет создано за {schedule.DaysCountBeforeNotificaiton} дней до дня рождения.{Environment.NewLine}Вы можете проверить список дней рождения воспользовавшись командой /birthdays";
                            await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: text, replyToMessageId: update.Message.MessageId);

                            telegramChat.LastCommandType = BirthdayNotificationScheduleBotCommandTypes.None;
                            await BirthdayScheduleTelegramBotRepository.UpdateChat(telegramChat);
                        }
                        catch (Exception ex)
                        {
                            await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Ошибка импорта данных", replyToMessageId: update.Message.MessageId);

                            throw;
                        }

                        break;
                    case BirthdayNotificationScheduleBotCommandTypes.SetNotificationDelayInDays:

                        if (telegramChat.BirthdayNotificationSchedules == null)
                            telegramChat.BirthdayNotificationSchedules = new List<BirthdaySchedule>();

                        if (byte.TryParse(update.Message.Text, out byte days))
                        {
                            if (!telegramChat.BirthdayNotificationSchedules.Any())
                            {
                                telegramChat.BirthdayNotificationSchedules.Add(new BirthdaySchedule
                                {
                                    DaysCountBeforeNotificaiton = days,
                                    TelegramChatId = telegramChat.Id
                                });
                            }
                            else
                            {
                                telegramChat.BirthdayNotificationSchedules.First().DaysCountBeforeNotificaiton = days;
                            }

                            telegramChat.LastCommandType = BirthdayNotificationScheduleBotCommandTypes.None;
                            await BirthdayScheduleTelegramBotRepository.UpdateChat(telegramChat);

                            await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Кол-во дней сохранено", replyToMessageId: update.Message.MessageId);
                        }
                        else
                        {
                            await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Неверный формат кол-ва дней", replyToMessageId: update.Message.MessageId);
                        }


                        break;
                    default:
                        await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Не удалось обработать команду", replyToMessageId: update.Message.MessageId);
                        break;
                }
            }
            else
            {
                telegramChat.LastCommandType = commandType.Value;
                await BirthdayScheduleTelegramBotRepository.UpdateChat(telegramChat);

                if (commandType == BirthdayNotificationScheduleBotCommandTypes.Birthdays)
                {
                    if (telegramChat.BirthdayNotificationSchedules == null || !telegramChat.BirthdayNotificationSchedules.Any() || (!telegramChat.BirthdayNotificationSchedules.First()?.Birthdays.Any() ?? false))
                    {
                        await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Вы еще не импортировали ни одного дня рождения", replyToMessageId: update.Message.MessageId);
                        return;
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine("Дни рождения:");
                    telegramChat.BirthdayNotificationSchedules.First().Birthdays.ForEach(x =>
                    {
                        sb.AppendLine($"{x.FirstName} {x.LastName} {x.DateOfBirth.ToString("dd.MM", CultureInfo.InvariantCulture)}");
                    });

                    await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: sb.ToString(), replyToMessageId: update.Message.MessageId);

                }
                else if (commandType == BirthdayNotificationScheduleBotCommandTypes.ClearBirthdays)
                {
                    telegramChat.BirthdayNotificationSchedules = new List<BirthdaySchedule>();
                    await BirthdayScheduleTelegramBotRepository.UpdateChat(telegramChat);

                    await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: $"Дни рождения успешно удалены{Environment.NewLine}Для импорта новых воспользуйтесь командой /import_birthdays", replyToMessageId: update.Message.MessageId);
                }
                else if (GetCommandTextMessage(telegramChat, commandType, update, out string text))
                {
                    await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: text, replyToMessageId: update.Message.MessageId, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                else
                {
                    await BotClient.SendTextMessageAsync(chatId: update.Message.Chat, text: "Не удалось обработать команду", replyToMessageId: update.Message.MessageId);
                }
            }
        }

        private bool GetCommandTextMessage(TelegramChat telegramChat, BirthdayNotificationScheduleBotCommandTypes? commandType, Update update, out string text)
        {
            text = null;

            switch (commandType.Value)
            {
                case BirthdayNotificationScheduleBotCommandTypes.SetNotificationDelayInDays:
                    text = $"За сколько дней нужно сообщать о дне рождения?{Environment.NewLine}Требуется указать цифру{Environment.NewLine}";
                    if (telegramChat.BirthdayNotificationSchedules != null && telegramChat.BirthdayNotificationSchedules.Any())
                        text += $"Текущее значение: {telegramChat.BirthdayNotificationSchedules.First().DaysCountBeforeNotificaiton}";
                    return true;
                case BirthdayNotificationScheduleBotCommandTypes.ImportBirthdays:
                    text = $@"Укажите список дней рождения в следующем формате:{Environment.NewLine}имя фамилия dd.MM{Environment.NewLine}имя фамилия dd.MM{Environment.NewLine}имя фамилия dd.MM{Environment.NewLine}и т.д.";
                    return true;
                default:
                    text = "Неизвестный тип команды";
                    return false;
            }
        }

        private BirthdayNotificationScheduleBotCommandTypes? GetCommandType(Update update)
        {
            if (string.IsNullOrWhiteSpace(update.Message.Text))
                return null;

            if (_commandNameToTypeMap.ContainsKey(update.Message.Text))
                return _commandNameToTypeMap[update.Message.Text];

            return null;
        }
    }
}