using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BirthdayNotificationService.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace BirthdayNotificationService.Persistence.Repositories
{
    public class BirthdayNotificationScheduleRepository
    {
        private readonly BirthdayNotificationScheduleDbContext _context;

        public BirthdayNotificationScheduleRepository(BirthdayNotificationScheduleDbContext context)
        {
            _context = context;
        }

        public async Task<TelegramChat> GetChat(long chatExternalId)
        {
            return await _context.TelegramChats
                .Include(x => x.BirthdayNotificationSchedules)
                .ThenInclude(x => x.Birthdays)
                .FirstOrDefaultAsync(x => x.ChatExternalId == chatExternalId);
        }

        public async Task<TelegramChat> AddChat(long chatExternalId, long userExternalId, string username)
        {
            var entry = await _context.AddAsync(new TelegramChat
            {
                ChatExternalId = chatExternalId,
                UserExternalId = userExternalId,
                Username = username
            });

            await _context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<List<TelegramChat>> Get()
        {
            return await _context.TelegramChats
                .Include(x => x.BirthdayNotificationSchedules)
                .ThenInclude(x => x.Birthdays)
                .ThenInclude(x => x.BirthdayNotificationsHistory)
                .ToListAsync();
        }

        public async Task UpdateChat(TelegramChat chat)
        {
            var entity = await _context.TelegramChats.FirstOrDefaultAsync(x => x.Id == chat.Id);
            if (entity == null)
                throw new Exception("Чат не найден");

            entity.LastCommandType = chat.LastCommandType;

            _context.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationHistory(BirthdayNotificationHistory birthdayNotificationHistory)
        {
            await _context.AddAsync(birthdayNotificationHistory);

            await _context.SaveChangesAsync();
        }
    }
}