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

namespace TrainFinder.WebJobs.RefreshTrains
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
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });

                    services.AddHttpClient<IBdzTimetableClient, BdzTimetableClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://razpisanie.bdz.bg/");
                        client.Timeout = TimeSpan.FromSeconds(30);
                    });
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var trainImportService = scope.ServiceProvider.GetRequiredService<ITrainImportService>();
                await trainImportService.RefreshTrainDataAsync(CancellationToken.None);
                logger.LogInformation("RefreshTrains WebJob completed successfully at {Time}.", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RefreshTrains WebJob failed at {Time}.", DateTime.UtcNow);
            }
        }
    }
}
