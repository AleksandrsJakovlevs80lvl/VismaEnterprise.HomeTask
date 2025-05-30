using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VismaEnterprise.HomeTask.Infrastructure.Authentication;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Options;
using VismaEnterprise.HomeTask.Web.Options;
using VismaEnterprise.HomeTask.Web.Services;

namespace VismaEnterprise.HomeTask.Web.Configurations;

public static class AuthenticationServices
{
    public static IServiceCollection AddHomeTaskAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
        var jwtOptions = jwtConfigurationSection.Get<JwtOptions>();

        services.AddOptions<JwtOptions>()
            .Bind(jwtConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<IPasswordManager, PasswordManager>();
        services.AddScoped<IAuthService, AuthService>();

        if (builder.Environment.IsDevelopment())
        {
            var adminConfigurationSection = builder.Configuration.GetSection("Admin");

            services.AddOptions<AdminOptions>()
                .Bind(adminConfigurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHostedService<EnsureAdminUserCreated>();
        }

        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        authBuilder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions!.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            };
        });

        return services;
    }
}