using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using TrainFinder.Application.Interfaces;
using TrainFinder.Application.Parsers;
using TrainFinder.Application.Parsers.Interfaces;
using TrainFinder.Application.Services;
using TrainFinder.Data.Context;
using TrainFinder.Infrastructure.Clients;
using TrainFinder.Infrastructure.Services;
using TrainFinder.Repository.Interfaces;
using TrainFinder.Repository.Repositories;
using TrainFinder.Web.Components;

namespace TrainFinder.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    builder.Configuration.Bind("AzureAd", options);

                    options.SignedOutCallbackPath = "/signout-callback-oidc";

                    options.Events.OnSignedOutCallbackRedirect = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.UiLocales = "bg-BG";
                        context.ProtocolMessage.SetParameter("mkt", "bg-BG");
                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        context.ProtocolMessage.UiLocales = "bg-BG";
                        context.ProtocolMessage.SetParameter("mkt", "bg-BG");
                        return Task.CompletedTask;
                    };

                    options.Events.OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    };
                })
                .EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
                .AddMicrosoftGraph()
                .AddInMemoryTokenCaches();

            builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            builder.Services.AddAuthorization();

            // Add services
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddDbContext<TrainFinderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TrainFinderDatabase")));

            builder.Services.AddScoped<ITrainRepository, TrainRepository>();
            builder.Services.AddScoped<IStationRepository, StationRepository>();
            builder.Services.AddScoped<ITrainLocationRepository, TrainLocationRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRequestRepository, RequestRepository>();

            builder.Services.AddScoped<ITrainService, TrainService>();
            builder.Services.AddScoped<IStationService, StationService>();
            builder.Services.AddScoped<ITrainLocationService, TrainLocationService>();
            builder.Services.AddScoped<ITrainImportService, TrainImportService>();
            builder.Services.AddScoped<ITimetableImportService, TimetableImportService>();
            builder.Services.AddScoped<ITimetableService, TimetableService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<IGraphUserService, GraphUserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IBdzRadarTrainParser, BdzRadarTrainParser>();
            builder.Services.AddScoped<IBdzTimetableParser, BdzTimetableParser>();

            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();

            builder.Services.AddHttpClient<IBdzRadarClient, BdzRadarClient>(client =>
            {
                client.BaseAddress = new Uri("https://radar.bdz.bg/");
            });

            builder.Services.AddHttpClient<IBdzTimetableClient, BdzTimetableClient>(client =>
            {
                client.BaseAddress = new Uri("https://razpisanie.bdz.bg/");
            });

            var app = builder.Build();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapControllers();

            app.Run();
        }
    }
}
