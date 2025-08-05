using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AccountService.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services, Uri authorizationUrl)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = authorizationUrl,
                        Scopes = new Dictionary<string, string>()
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
                        }
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Name = "Bearer",
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
                },
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    public static void AddKeycloakAuthentication(this IServiceCollection services, string authScheme,
        string audience, string metadataAddress, string validIssuer)
    {
        services.AddAuthentication(authScheme).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.Audience = audience;
            //x.Authority = authority;

            x.MetadataAddress = metadataAddress;

            x.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidIssuer = validIssuer,
                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                {
                    if (expires != null)
                        return expires.Value > DateTime.UtcNow;

                    return false;
                },
            };
        });
    }
}