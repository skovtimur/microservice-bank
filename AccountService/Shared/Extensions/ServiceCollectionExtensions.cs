using System.Reflection;
using AccountService.Features.Transactions.Domain;
using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Abstractions;
using AccountService.Shared.Abstractions.BackgroundJobInterfaces;
using AccountService.Shared.Abstractions.ServiceInterfaces;
using AccountService.Shared.BackgroundJobs;
using AccountService.Shared.Infrastructure.Repositories;
using AccountService.Shared.Options;
using AccountService.Shared.RabbitMq.Consumers;
using AccountService.Shared.RabbitMq.RabbitMqEvents;
using AccountService.Shared.Services;
using AccountService.Shared.Validators;
using MassTransit;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace AccountService.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    // ReSharper disable UnusedMethodReturnValue.Global
    public static IServiceCollection AddMyOptions(this IServiceCollection services, ConfigurationManager conf)
    {
        services.AddOptions<RabbitMqOptions>()
            .Bind(conf.GetRequiredSection("RabbitMq"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped<IInterestAccrualDailyBackgroundJob, InterestAccrualDailyBackgroundJob>();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, Uri authorizationUrl)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = authorizationUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
                        }
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Bearer"
                    },
                    []
                }
            });

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Timur",
                    Url = new Uri("https://t.me/skovtimur")
                }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        return services;
    }

    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, string authScheme,
        string audience, string metadataAddress, string validIssuer)
    {
        services.AddAuthentication(authScheme).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.Audience = audience;
            //x.Authority = authority;

            x.MetadataAddress = metadataAddress;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidIssuer = validIssuer,
                LifetimeValidator = (_, expires, _, _) =>
                {
                    if (expires != null)
                        return expires.Value > DateTime.UtcNow;

                    return false;
                }
            };
        });
        return services;
    }

    public static IServiceCollection AddMasstransitMessaging(this IServiceCollection services, RabbitMqOptions options)
    {
        var exchangeName = options.ExchangeName;

        var factory = new ConnectionFactory
        {
            Port = options.Port,
            UserName = options.User,
            Password = options.Password,
            HostName = options.Host
        };

        const int maxRetries = 10;
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                using var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
                channel.ExchangeDeclareAsync(exchangeName, "topic", durable: true, autoDelete: false).GetAwaiter()
                    .GetResult();

                // account.crm queue:
                channel.QueueDeclareAsync(options.CrmQueue, durable: true, exclusive: false, autoDelete: false)
                    .GetAwaiter()
                    .GetResult();
                channel.QueueBindAsync(options.CrmQueue, exchangeName, options.AccountKeyPattern).GetAwaiter()
                    .GetResult();

                // notification queue:
                channel.QueueDeclareAsync(options.NotificationsQueue, durable: true, exclusive: false,
                        autoDelete: false)
                    .GetAwaiter().GetResult();
                channel.QueueBindAsync(options.NotificationsQueue, exchangeName, options.MoneyKeyPattern).GetAwaiter()
                    .GetResult();
            }
            catch (BrokerUnreachableException)
            {
                if (i == maxRetries - 1)
                    throw;

                Thread.Sleep(3000);

                //Rabbit-у нуюно проснутся, это занимает время, странно звучит...
                //Вобщем, Ему нужно пару секунд чтобы инициализировать все что нужно у себя
                //Быавет мой клиент пытается до него достучаться раньше чем он проснулся, я даже не про старт сервиса в docker,
                //то есть контейнер с Rabbit может docker запустил 2 секунды назад, а тот досихпор не в рабочем состоянии. 
            }
        }

        services.AddMassTransit(c =>
        {
            c.SetKebabCaseEndpointNameFormatter();

            c.AddConsumer<ClientBlockedConsumer>();
            c.AddConsumer<ClientUnblockedConsumer>();

            c.UsingRabbitMq((context, cfg) =>
            {
                //To Connect
                cfg.Host(options.Host, "/", h =>
                {
                    h.Username(options.User);
                    h.Password(options.Password);
                });

                //Messages:
                cfg.Message<AccountOpenedEventModel>(x => x.SetEntityName(exchangeName));
                cfg.Message<InterestAccruedEventModel>(x => x.SetEntityName(exchangeName));
                cfg.Message<MoneyCreditedEventModel>(x => x.SetEntityName(exchangeName));
                cfg.Message<MoneyDebitedEventModel>(x => x.SetEntityName(exchangeName));
                cfg.Message<ClientBlockedEventModel>(x => x.SetEntityName(exchangeName));
                cfg.Message<ClientUnblockedEventModel>(x => x.SetEntityName(exchangeName));

                cfg.Publish<BaseRabbitEvent>(x => x.Exclude = true);
                cfg.Publish<AccountOpenedEventModel>(x => { x.ExchangeType = "topic"; });
                cfg.Publish<InterestAccruedEventModel>(x => { x.ExchangeType = "topic"; });
                cfg.Publish<MoneyCreditedEventModel>(x => { x.ExchangeType = "topic"; });
                cfg.Publish<MoneyDebitedEventModel>(x => { x.ExchangeType = "topic"; });
                cfg.Publish<ClientBlockedEventModel>(x => { x.ExchangeType = "topic"; });
                cfg.Publish<ClientUnblockedEventModel>(x => { x.ExchangeType = "topic"; });


                //anti-fraud queue:
                //в целом как раз consumer пишет конфигурацию для queue
                cfg.Send<AccountOpenedEventModel>(x => x.UseRoutingKeyFormatter(_ => options.AccountOpenedRoutingKey));
                cfg.Send<InterestAccruedEventModel>(x => x.UseRoutingKeyFormatter(_ => options.InterestAccruedRoutingKey));
                cfg.Send<MoneyCreditedEventModel>(x => x.UseRoutingKeyFormatter(_ => options.MoneyCreditedRoutingKey));
                cfg.Send<MoneyDebitedEventModel>(x => x.UseRoutingKeyFormatter(_ => options.MoneyDebitedRoutingKey));

                cfg.Send<ClientBlockedEventModel>(x => x.UseRoutingKeyFormatter(_ => options.ClientBlockedRoutingKey));
                cfg.Send<ClientUnblockedEventModel>(x =>
                    x.UseRoutingKeyFormatter(_ => options.ClientUnblockedRoutingKey));

                cfg.ReceiveEndpoint(options.AntifraudQueue, e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.ExchangeType = "topic";
                    e.Durable = true;

                    e.Bind(exchangeName, x =>
                    {
                        x.Durable = true;
                        x.ExchangeType = "topic";
                        x.RoutingKey = options.ClientKeyPattern;
                    });

                    e.ConfigureConsumer<ClientBlockedConsumer>(context);
                    e.ConfigureConsumer<ClientUnblockedConsumer>(context);
                    e.Bind<ClientBlockedEventModel>(x =>
                    {
                        x.Durable = true;
                        x.ExchangeType = "topic";
                        x.RoutingKey = options.ClientBlockedRoutingKey;
                    });
                    e.Bind<ClientUnblockedEventModel>(x =>
                    {
                        x.Durable = true;
                        x.ExchangeType = "topic";
                        x.RoutingKey = options.ClientUnblockedRoutingKey;
                    });
                });

                cfg.ClearSerialization();
                cfg.UseRawJsonSerializer();
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}