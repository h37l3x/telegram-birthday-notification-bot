## Бот

Телеграм бот, который умеет уведомлять о предстоящих днях рождения

название - **birthday_schedule**

логин - **@birthday_schedule_bot**

## Список команд для бота

**set_notification_delay_in_days** - Указать за сколько дней до дня рождения будет создан чат

**import_birthdays** - Импорт дней рождения

**birthdays** - список дней рождения

**clear_birthdays** - очистка списка дней рождения для уведомлений

## Описание работы

Из за отсутствия отдельного домена на **https**, работа с **основана на опросах сервиса Telegram** о новых поступивших сообщениях от пользователей

Бот реализован на основе **Hangfire** с публичными методами для запуска задач
1. задача на периодический опрос о новых обновлениях
2. задача на отправку уведомлений о днях рождения

Задачами можно управлять напрямую из данного интерфейса т.к. поддержана логика обработки токенов отмены **CancellationToken**, 
т.е. при отмене задачи через UI она будет остановлена и в коде (даже если выполняется бесконечный цикл опросов сервиса Telegram)

## Настройки

    {
        "Token": "",
        "BirthdaySheduleTelegramBotToken": "",
        "Socks5Hostname": "",
        "Socks5Port": "",
        "Socks5Username": "",
        "Socks5Password":  ""
    },

## Методы API

1. Для запуска задачи на получение и обработку пользовательских сообщений

```
/api/bot/telegram/birthday-schedule/start-bot-settings-job
```

тело запроса
```
{
	"token": "токен безопасности"
}
```

2. для создания задачи на отправку уведомлений по расписанию

```
/api/bot/telegram/birthday-schedule/start-bot-settings-job
```

тело запроса
```
{
	"token": "токен безопасности",
	"HoursUTC": 9
}
```

**HoursUTC** - время в часах в UTC, в которое будет выполнена задача

*При повторном вызове метода новой задачи (дубликата) не создается*

# Предварительные требования:

1. NET Core SDK v2.2.2 Можно скачать отсюда: https://dotnet.microsoft.com/download

# Работа с [.NET Core CLI](https://docs.microsoft.com/dotnet/core/tools)

## Сборка

Из директории проекта выполнить команду [dotnet build](https://docs.microsoft.com/dotnet/core/tools/dotnet-build)

```
dotnet build
```

## Запуск тестов

Из директории проекта выполнить команду [dotnet test](https://docs.microsoft.com/dotnet/core/tools/dotnet-test)

```
dotnet test
```

## Запуск сервиса

Из директории проекта выполнить команду [dotnet run](https://docs.microsoft.com/dotnet/core/tools/dotnet-run)

Запуск с окружением Development:
```
dotnet run --project "./BirthdayNotificationService.Application.WebApi/BirthdayNotificationService.Application.WebApi.csproj" --launch-profile "Development"
```

## Публикация

Из директории проекта выполнить команду [dotnet publish](https://docs.microsoft.com/dotnet/core/tools/dotnet-publish)

Тестовый Stage

```
dotnet publish "./BirthdayNotificationService.Application.WebApi/BirthdayNotificationService.Application.WebApi.csproj" -c Debug --output "путь до папки" /p:EnvironmentName=Development
```

**EnvironmentName** может принимать значения **Development**, **Staging** или **Production**

## Работа с БД

Для успешного выполнения команд необходимо, чтобы текущей директорией был проект **BirthdayNotificationService.Application.WebApi**

Строка подключения настраивается в appsettings.json проекта **BirthdayNotificationService.Application.WebApi**
```
    "ConnectionStrings": {
        "BirthdayNotificationSchedule": "Server=PROG21\\SQLEXPRESS;Database=ExampleDatabase;Trusted_Connection=True"
    }
```

Добавление новой миграции
```
dotnet ef migrations add InitialMigration --project ../BirthdayNotificationService.Persistence
```

Обновление БД
```
dotnet ef database update --project ../BirthdayNotificationService.Persistence
```