using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BirthdayNotificationService.Domain.Contracts.Services
{
    public interface IBirthdayNotificationService
    {
        Task ProccessMessage(Update update);
    }
}