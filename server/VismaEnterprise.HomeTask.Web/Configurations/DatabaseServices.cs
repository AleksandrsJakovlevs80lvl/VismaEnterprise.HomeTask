using Microsoft.EntityFrameworkCore;
using VismaEnterprise.HomeTask.Domain.Repositories;
using VismaEnterprise.HomeTask.Infrastructure.Database;
using VismaEnterprise.HomeTask.Infrastructure.Database.Repositories;
using VismaEnterprise.HomeTask.Web.Options;

namespace VismaEnterprise.HomeTask.Web.Configurations;

public static class DatabaseServices
{
    public static IServiceCollection AddHomeTaskDatabase(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var databaseConfigurationSection = builder.Configuration.GetSection("ConnectionStrings");
        var databaseOptions = databaseConfigurationSection.Get<DatabaseOptions>();

        services.AddOptions<DatabaseOptions>()
            .Bind(databaseConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<ICatalogueEntryRepository, CatalogueEntryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookRepository, BookRepository>();

        services.AddDbContext<HomeTaskDbContext>(options =>
            options.UseMySql(databaseOptions!.DefaultConnection, ServerVersion.AutoDetect(databaseOptions.DefaultConnection)));

        return services;
    }
}