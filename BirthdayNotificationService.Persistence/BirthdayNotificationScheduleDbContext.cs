using BirthdayNotificationService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BirthdayNotificationService.Persistence
{
    public class BirthdayNotificationScheduleDbContext : DbContext
    {
        public BirthdayNotificationScheduleDbContext(DbContextOptions<BirthdayNotificationScheduleDbContext> options)
           : base(options)
        { }

        public DbSet<TelegramChat> TelegramChats { get; set; }
        public DbSet<BirthdaySchedule> BirthdayNotificationSchedules { get; set; }
        public DbSet<Birthday> Birthdays { get; set; }
        public DbSet<BirthdayNotificationHistory> BirthdayNotificationHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TelegramChat>().HasKey(x => x.Id);
            modelBuilder.Entity<TelegramChat>().HasIndex(x => x.ChatExternalId).IsUnique();
            modelBuilder.Entity<TelegramChat>().Property(x => x.UserExternalId).IsRequired();
            modelBuilder.Entity<TelegramChat>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<TelegramChat>().Property(x => x.Username).IsRequired();
            modelBuilder.Entity<TelegramChat>().Property(x => x.LastCommandType).IsRequired();
            modelBuilder.Entity<TelegramChat>().HasMany(x => x.BirthdayNotificationSchedules).WithOne(x => x.TelegramChat);

            modelBuilder.Entity<BirthdaySchedule>().HasKey(x => x.Id);
            modelBuilder.Entity<BirthdaySchedule>().HasIndex(x => x.TelegramChatId).IsUnique();
            modelBuilder.Entity<BirthdaySchedule>().Property(x => x.TelegramChatId).IsRequired();
            modelBuilder.Entity<BirthdaySchedule>().Property(x => x.DaysCountBeforeNotificaiton).IsRequired();
            modelBuilder.Entity<BirthdaySchedule>().HasOne(x => x.TelegramChat).WithMany(x => x.BirthdayNotificationSchedules);
            modelBuilder.Entity<BirthdaySchedule>().HasMany(x => x.Birthdays).WithOne(x => x.BirthdayNotificationSchedule);

            modelBuilder.Entity<Birthday>().HasKey(x => x.Id);
            modelBuilder.Entity<Birthday>().Property(x => x.FirstName).IsRequired();
            modelBuilder.Entity<Birthday>().Property(x => x.LastName).IsRequired();
            modelBuilder.Entity<Birthday>().Property(x => x.DateOfBirth).IsRequired();
            modelBuilder.Entity<Birthday>().Property(x => x.BirthdayNotificationScheduleId).IsRequired();
            modelBuilder.Entity<Birthday>().HasOne(x => x.BirthdayNotificationSchedule).WithMany(x => x.Birthdays);
            modelBuilder.Entity<Birthday>().HasMany(x => x.BirthdayNotificationsHistory).WithOne(x => x.Birthday);

            modelBuilder.Entity<BirthdayNotificationHistory>().HasKey(x => x.Id);
            modelBuilder.Entity<BirthdayNotificationHistory>().Property(x => x.CreationDate).IsRequired();
            modelBuilder.Entity<BirthdayNotificationHistory>().Property(x => x.NotificationYear).IsRequired();
            modelBuilder.Entity<BirthdayNotificationHistory>().HasOne(x => x.Birthday).WithMany(x => x.BirthdayNotificationsHistory);
            modelBuilder.Entity<BirthdayNotificationHistory>().HasIndex(x => new { x.NotificationYear, x.BirthdayId }).IsUnique();
        }
    }
}