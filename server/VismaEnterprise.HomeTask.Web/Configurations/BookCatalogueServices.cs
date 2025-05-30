using VismaEnterprise.HomeTask.Application.Mappers;
using VismaEnterprise.HomeTask.Application.Mappers.Interfaces;
using VismaEnterprise.HomeTask.Application.Services;
using VismaEnterprise.HomeTask.Application.Services.Interfaces;
using VismaEnterprise.HomeTask.Domain.Services;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;
using VismaEnterprise.HomeTask.Web.Services;

namespace VismaEnterprise.HomeTask.Web.Configurations;

public static class BookCatalogueServices
{
    public static IServiceCollection AddBookCatalogue(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextResolver, UserContextResolver>();

        services.AddScoped<ICatalogueEntryMapper, CatalogueEntryMapper>();

        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<ICatalogueEntryService, CatalogueEntryService>();

        services.AddScoped<IBookCatalogueService, BookCatalogueService>();

        return services;
    }
}