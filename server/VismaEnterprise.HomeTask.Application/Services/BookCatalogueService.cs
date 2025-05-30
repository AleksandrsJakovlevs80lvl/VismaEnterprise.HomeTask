using VismaEnterprise.HomeTask.Application.DTOs;
using VismaEnterprise.HomeTask.Application.Mappers.Interfaces;
using VismaEnterprise.HomeTask.Application.Services.Interfaces;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;

namespace VismaEnterprise.HomeTask.Application.Services;

public class BookCatalogueService(IUserContextResolver contextResolver, ICatalogueEntryService catalogueEntryService, ICatalogueEntryMapper catalogueEntryMapper) : IBookCatalogueService
{
    public async Task<List<CatalogueEntryDto>> GetEntriesAsync()
    {
        var userContext = contextResolver.Resolve();
        if (userContext == null)
        {
            throw new InvalidOperationException("Can not resolve user context.");
        }

        var entries = await catalogueEntryService.GetEntriesAsync(userContext.UserAccountId, userContext.IsAdmin);

        return entries.Select(catalogueEntryMapper.Map).ToList();
    }

    public async Task<CatalogueEntryDto?> GetEntryAsync(Guid id)
    {
        var userContext = contextResolver.Resolve();
        if (userContext == null)
        {
            throw new InvalidOperationException("Can not resolve user context.");
        }

        var entry = await catalogueEntryService.GetEntryAsync(userContext.UserAccountId, userContext.IsAdmin, id);
        if (entry == null)
        {
            return null;
        }

        return catalogueEntryMapper.Map(entry);
    }

    public async Task<CatalogueEntryDto?> EditEntryAsync(Guid id, CatalogueEntryDto entryDto)
    {
        var userContext = contextResolver.Resolve();
        if (userContext == null)
        {
            throw new InvalidOperationException("Can not resolve user context.");
        }

        var genre = catalogueEntryMapper.MapGenre(entryDto.Genre);

        var entry = await catalogueEntryService.UpdateEntryAsync(userContext.UserAccountId, userContext.IsAdmin, id, entryDto.PublishedYear, genre, entryDto.Mark);
        if (entry == null)
        {
            return null;
        }

        return catalogueEntryMapper.Map(entry);
    }

    public async Task<CatalogueEntryDto> CreateEntryAsync(CatalogueEntryDto entryDto)
    {
        var userContext = contextResolver.Resolve();
        if (userContext == null)
        {
            throw new InvalidOperationException("Can not resolve user context.");
        }

        var genre = catalogueEntryMapper.MapGenre(entryDto.Genre);

        var entry = await catalogueEntryService.CreateEntryAsync(userContext.UserAccountId, entryDto.Title, entryDto.Author, entryDto.PublishedYear, genre, entryDto.Mark);

        return catalogueEntryMapper.Map(entry);
    }

    public async Task<CatalogueEntryDto?> DeleteEntryAsync(Guid id)
    {
        var userContext = contextResolver.Resolve();
        if (userContext == null)
        {
            throw new InvalidOperationException("Can not resolve user context.");
        }

        var entry = await catalogueEntryService.DeleteEntryAsync(userContext.UserAccountId, userContext.IsAdmin, id);
        if (entry == null)
        {
            return null;
        }

        return catalogueEntryMapper.Map(entry);
    }
}