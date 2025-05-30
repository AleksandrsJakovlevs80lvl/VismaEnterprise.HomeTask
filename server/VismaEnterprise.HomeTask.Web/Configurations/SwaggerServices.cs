using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VismaEnterprise.HomeTask.Web.Configurations;

public static class SwaggerServices
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}

public class ConfigureSwaggerOptions(IConfiguration configuration) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeTaskApi", Version = "v1" });

        options.AddServer(new OpenApiServer
        {
            Url = configuration["Docs:ServerUrl"],
            Description = "Docs Server"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    }
}