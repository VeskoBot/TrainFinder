using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrainFinder.Application.Interfaces;
using TrainFinder.Application.Parsers;
using TrainFinder.Application.Parsers.Interfaces;
using TrainFinder.Application.Services;
using TrainFinder.Data.Context;
using TrainFinder.Infrastructure.Clients;
using TrainFinder.Repository.Interfaces;
using TrainFinder.Repository.Repositories;

namespace TrainFinder.LocalRunner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("TrainFinderDatabase");

                    services.AddDbContext<TrainFinderDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddScoped<ITrainRepository, TrainRepository>();
                    services.AddScoped<IStationRepository, StationRepository>();
                    services.AddScoped<ITrainLocationRepository, TrainLocationRepository>();
                    services.AddScoped<ITimetableRepository, TimetableRepository>();
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<IRoleRepository, RoleRepository>();
                    services.AddScoped<IRequestRepository, RequestRepository>();

                    services.AddScoped<ITrainService, TrainService>();
                    services.AddScoped<IStationService, StationService>();
                    services.AddScoped<ITrainLocationService, TrainLocationService>();
                    services.AddScoped<ITimetableService, TimetableService>();
                    services.AddScoped<ITrainImportService, TrainImportService>();
                    services.AddScoped<ITimetableImportService, TimetableImportService>();

                    services.AddScoped<IBdzRadarTrainParser, BdzRadarTrainParser>();
                    services.AddScoped<IBdzTimetableParser, BdzTimetableParser>();

                    services.AddHttpClient<IBdzRadarClient, BdzRadarClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://radar.bdz.bg/");
                        client.Timeout = TimeSpan.FromSeconds(60);
                    });

                    services.AddHttpClient<IBdzTimetableClient, BdzTimetableClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://razpisanie.bdz.bg/");
                        client.Timeout = TimeSpan.FromSeconds(60);
                    });
                })
                .Build();

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogInformation("TrainFinder LocalRunner started. Press Ctrl+C to stop.");

            await RunTimetableRefresh(host.Services, logger);

            var lastTimetableRun = DateTime.UtcNow;

            while (!cts.Token.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var secondsUntilNextFive = (5 - (now.Minute % 5)) * 60 - now.Second;
                var nextRun = TimeSpan.FromSeconds(secondsUntilNextFive);

                logger.LogInformation("Next train refresh in {Seconds}s at {Time}.", (int)nextRun.TotalSeconds, now.Add(nextRun).ToString("HH:mm"));

                await Task.Delay(nextRun, cts.Token).ContinueWith(_ => { });

                if (cts.Token.IsCancellationRequested)
                    break;

                await RunTrainRefresh(host.Services, logger);

                if (DateTime.UtcNow - lastTimetableRun >= TimeSpan.FromHours(24))
                {
                    await RunTimetableRefresh(host.Services, logger);
                    lastTimetableRun = DateTime.UtcNow;
                }
            }

            logger.LogInformation("TrainFinder LocalRunner stopped.");
        }

        private static async Task RunTrainRefresh(IServiceProvider services, ILogger logger)
        {
            using var scope = services.CreateScope();
            try
            {
                logger.LogInformation("Running train refresh at {Time}.", DateTime.UtcNow.ToString("HH:mm:ss"));
                var trainImportService = scope.ServiceProvider.GetRequiredService<ITrainImportService>();
                await trainImportService.RefreshTrainDataAsync(CancellationToken.None);
                logger.LogInformation("Train refresh completed at {Time}.", DateTime.UtcNow.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Train refresh failed.");
            }
        }

        private static async Task RunTimetableRefresh(IServiceProvider services, ILogger logger)
        {
            using var scope = services.CreateScope();
            try
            {
                logger.LogInformation("Running timetable refresh at {Time}.", DateTime.UtcNow.ToString("HH:mm:ss"));
                var timetableImportService = scope.ServiceProvider.GetRequiredService<ITimetableImportService>();
                await timetableImportService.ImportAllTimetablesAsync(CancellationToken.None);
                logger.LogInformation("Timetable refresh completed at {Time}.", DateTime.UtcNow.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Timetable refresh failed.");
            }
        }
    }
}
