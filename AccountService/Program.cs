using System.Text.Json.Serialization;
using AccountService.Features.Wallets.CreateWallet;
using AccountService.Shared.Api.Filters;
using AccountService.Shared.Extensions;
using AccountService.Shared.Infrastructure;
using AccountService.Shared.Mapper;
using AccountService.Shared.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace AccountService;

public class Program
{
    private const string MainAuthScheme = JwtBearerDefaults.AuthenticationScheme;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<MainDbContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("account-service-db").GetRequiredString();

            opt.UseNpgsql(connectionString);
            opt.UseLoggerFactory(MainDbContext.GetLoggerFactory);
        });
        builder.Services.AddMyOptions(builder.Configuration);
        builder.Services.AddSwagger(new Uri(builder.Configuration["Keycloak:AuthorizationUrl"] ??
                                            throw new NullReferenceException("Keycloak:AuthorizationUrl is empty")));
        builder.Services.AddRouting();
        builder.Services.AddCors(x =>
            x.AddPolicy("main_policy",
                c => c.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

        builder.Services.AddKeycloakAuthentication(MainAuthScheme,
            builder.Configuration["Keycloak:Audience"].GetRequiredString(),
            builder.Configuration["Keycloak:MetadataAddress"].GetRequiredString(),
            builder.Configuration["Keycloak:ValidIssuer"].GetRequiredString());
        builder.Services.AddAuthorization();

        if (builder.Environment.IsTestEnvironment() == false)
        {
            var rabbitMqOptions = builder.Configuration.GetRequiredSection("RabbitMq").Get<RabbitMqOptions>(options =>
                                  {
                                      options.ErrorOnUnknownConfiguration = true;
                                  }) ??
                                  throw new NullReferenceException("RabbitMq configuration is null");
            builder.Services.AddMasstransitMessaging(rabbitMqOptions);

            builder.Services.AddHangfire(configuration =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                configuration.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("hangfire-db"));
#pragma warning restore CS0618 // Type or member is obsolete
            });
            builder.Services.AddHangfireServer();
            builder.AddHangfireDb(); // Если бд нет он его создаст}
        }

        builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MainMapper>(); });
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateWalletCommandValidator>(
            ServiceLifetime.Transient);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateWalletCommandHandler).Assembly);
        });
        builder.Services.AddMyServices();

        var app = builder.Build();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (app.Environment.IsTestEnvironment() == false)
        {
            app.UseRecurrentJobs();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => { x.OAuthClientId(app.Configuration["Keycloak:ClientId"]); });
            app.UseHangfireDashboard("/hangfire-dashboard", new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new AllowAllHangfireDashboardAuthorizationFilter()
                }
            });

            app.MapOpenApi();
            app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
                .ExcludeFromDescription();
        }

        app.ApplyMigrations();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}