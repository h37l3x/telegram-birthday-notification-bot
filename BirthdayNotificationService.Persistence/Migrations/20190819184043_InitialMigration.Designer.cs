﻿// <auto-generated />
using System;
using BirthdayNotificationService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BirthdayNotificationService.Persistence.Migrations
{
    [DbContext(typeof(BirthdayNotificationScheduleDbContext))]
    [Migration("20190819184043_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BirthdayNotificationService.Persistence.Entities.Birthday", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("BirthdayNotificationScheduleId");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BirthdayNotificationScheduleId");

                    b.ToTable("BirthdayNotificationScheduleUsers");
                });

            modelBuilder.Entity("BirthdayNotificationService.Persistence.Entities.BirthdaySchedule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("DaysCountBeforeNotificaiton");

                    b.Property<string>("NotificationChatWelcomeMessage");

                    b.Property<long>("TelegramChatId");

                    b.HasKey("Id");

                    b.HasIndex("TelegramChatId")
                        .IsUnique();

                    b.ToTable("BirthdayNotificationSchedules");
                });

            modelBuilder.Entity("BirthdayNotificationService.Persistence.Entities.TelegramChat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ChatExternalId");

                    b.Property<int>("LastCommandType");

                    b.Property<long>("UserExternalId");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ChatExternalId")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("TelegramChats");
                });

            modelBuilder.Entity("BirthdayNotificationService.Persistence.Entities.Birthday", b =>
                {
                    b.HasOne("BirthdayNotificationService.Persistence.Entities.BirthdaySchedule", "BirthdayNotificationSchedule")
                        .WithMany("Birthdays")
                        .HasForeignKey("BirthdayNotificationScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BirthdayNotificationService.Persistence.Entities.BirthdaySchedule", b =>
                {
                    b.HasOne("BirthdayNotificationService.Persistence.Entities.TelegramChat", "TelegramChat")
                        .WithMany("BirthdayNotificationSchedules")
                        .HasForeignKey("TelegramChatId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
