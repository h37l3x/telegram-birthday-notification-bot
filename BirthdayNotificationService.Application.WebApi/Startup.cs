using System;
using System.IO;
using System.Net;
using System.Net.Http;
using BirthdayNotificationService.Application.Handlers.Commands;
using BirthdayNotificationService.Common.ConfigOptions;
using BirthdayNotificationService.Domain.Services.BirthdayNotificationServices;
using BirthdayNotificationService.Persistence;
using BirthdayNotificationService.Persistence.Repositories;
using ElmahCore;
using ElmahCore.Mvc;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace BirthdayNotificationService.Application.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("BirthdayNotificationSchedule");
            services.AddDbContext<BirthdayNotificationScheduleDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("BirthdayNotificationService.Persistence")));

            services.AddOptions();
            services.Configure<AuthOptions>(Configuration.GetSection("AuthOptions"));

            services.AddSingleton<ErrorLog>(x => new XmlFileErrorLog(x.GetService<IOptions<ElmahOptions>>(), x.GetService<IHostingEnvironment>()));
            services.AddSingleton(x => GetTelegramBotClient(x));
            services.AddScoped<BirthdayScheduleTelegramBotJobsHandler>();
            services.AddScoped<BirthdayService>();
            services.AddScoped<BirthdayNotificationsService>();
            services.AddScoped<BirthdayNotificationScheduleRepository>();

            services.AddHangfire(x =>
            {
                var sqlOptions = new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                };

                x.UseSqlServerStorage(connectionString, sqlOptions);
            });

            services.AddHangfireServer(x =>
            {
                x.WorkerCount = 2;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddElmah<XmlFileErrorLog>(o =>
            {
                o.LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/elmah");
            });
        }

        private ITelegramBotClient GetTelegramBotClient(IServiceProvider serviceProvider)
        {
            var authOptionsAccessor = serviceProvider.GetService<IOptionsMonitor<AuthOptions>>();
            var authOptions = authOptionsAccessor.CurrentValue;
            var proxy = new HttpToSocks5Proxy(authOptions.Socks5Hostname, authOptions.Socks5Port, authOptions.Socks5Username, authOptions.Socks5Password);
            proxy.ResolveHostnamesLocally = true;

            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true,
                UseDefaultCredentials = false
            };
            var httpClient = new HttpClient(handler, true);

            var botClient = new TelegramBotClient(authOptions.BirthdaySheduleTelegramBotToken, httpClient);
            return botClient;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 0
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            app.UseMvc();

            app.UseElmah();
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}