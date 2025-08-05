using System.Text.Json.Serialization;
using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Commands.CreateWallet;
using AccountService.Extensions;
using AccountService.Services;
using AccountService.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AccountService;

public static class Program
{
    private const string MainAuthScheme = JwtBearerDefaults.AuthenticationScheme;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddSwagger(new Uri(builder.Configuration["Keycloak:AuthorizationUrl"] ??
                                            throw new NullReferenceException("Keycloak:AuthorizationUrl is empty")));
        builder.Services.AddRouting();
        builder.Services.AddCors(x =>
            x.AddPolicy("main_policy",
                c => c.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

        builder.Services.AddScoped<IClaimsService, ClaimsService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        builder.Services.AddKeycloakAuthentication(MainAuthScheme,
            StringExtensions.GetRequiredString(builder.Configuration["Keycloak:Audience"]),
            StringExtensions.GetRequiredString(builder.Configuration["Keycloak:MetadataAddress"]),
            StringExtensions.GetRequiredString(builder.Configuration["Keycloak:ValidIssuer"]));
        builder.Services.AddAuthorization();

        builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MainMapper>(); });
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateWalletCommandValidator>(ServiceLifetime.Transient);

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateWalletCommandHandler).Assembly);
        });
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => { x.OAuthClientId(app.Configuration["Keycloak:ClientId"]); });

            app.MapOpenApi();
            app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
                .ExcludeFromDescription();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}