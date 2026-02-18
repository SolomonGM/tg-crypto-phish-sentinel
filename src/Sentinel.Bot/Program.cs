using Microsoft.EntityFrameworkCore;
using Serilog;
using Sentinel.Bot.Configuration;
using Sentinel.Bot.Data;
using Sentinel.Bot.Services;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, config) =>
    {
        config
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    })
    .ConfigureServices((context, services) =>
    {
        var settings = RuntimeSettings.FromConfiguration(context.Configuration);
        services.AddSingleton(settings);

        services.AddDbContext<SentinelDbContext>(options =>
        {
            if (settings.UseSqlite)
            {
                options.UseSqlite(settings.SqliteConnectionString);
            }
            else
            {
                options.UseNpgsql(settings.BuildPostgresConnectionString());
            }
        });

        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(settings.TelegramBotToken));

        services.AddSingleton<IMessageScanner, HeuristicMessageScanner>();
        services.AddSingleton<IRecentAddressCache, RecentAddressCache>();
        services.AddSingleton<IRateLimiter, InMemoryRateLimiter>();
        services.AddSingleton<IScanReportFormatter, ScanReportFormatter>();

        services.AddScoped<IChatConfigurationService, ChatConfigurationService>();
        services.AddScoped<UpdateProcessor>();

        services.AddHostedService<DatabaseMigrationHostedService>();
        services.AddHostedService<TelegramPollingService>();
    })
    .Build();

await host.RunAsync();
