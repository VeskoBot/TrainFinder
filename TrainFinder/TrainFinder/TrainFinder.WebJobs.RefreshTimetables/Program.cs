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

namespace TrainFinder.WebJobs.RefreshTimetables
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
                    services.AddScoped<ITimetableImportService, TimetableImportService>();

                    services.AddScoped<IBdzTimetableParser, BdzTimetableParser>();

                    var proxyUrl = context.Configuration["BdzProxyUrl"];

                    services.AddHttpClient<IBdzTimetableClient, BdzTimetableClient>(client =>
                    {
                        client.BaseAddress = new Uri("http://razpisanie.bdz.bg/");
                        client.Timeout = TimeSpan.FromSeconds(60);
                    }).ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var handler = new HttpClientHandler();
                        if (!string.IsNullOrEmpty(proxyUrl))
                        {
                            handler.Proxy = new System.Net.WebProxy(proxyUrl);
                            handler.UseProxy = true;
                        }
                        return handler;
                    });
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var timetableImportService = scope.ServiceProvider.GetRequiredService<ITimetableImportService>();
                await timetableImportService.ImportAllTimetablesAsync(CancellationToken.None);
                logger.LogInformation("RefreshTimetables WebJob completed successfully at {Time}.", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RefreshTimetables WebJob failed at {Time}.", DateTime.UtcNow);
            }
        }
    }
}
