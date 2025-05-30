using VismaEnterprise.HomeTask.Application.DTOs;

namespace VismaEnterprise.HomeTask.Application.Services.Interfaces;

public interface IBookCatalogueService
{
    Task<List<CatalogueEntryDto>> GetEntriesAsync();
    Task<CatalogueEntryDto?> GetEntryAsync(Guid id);
    Task<CatalogueEntryDto?> EditEntryAsync(Guid id, CatalogueEntryDto entryDto);
    Task<CatalogueEntryDto> CreateEntryAsync(CatalogueEntryDto entryDto);
    Task<CatalogueEntryDto?> DeleteEntryAsync(Guid id);
}