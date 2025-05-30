using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Services.Interfaces;

public interface ICatalogueEntryService
{
    Task<List<CatalogueEntry>> GetEntriesAsync(Guid userAccountId, bool isAdmin);
    Task<CatalogueEntry?> GetEntryAsync(Guid userContextUserAccountId, bool userContextIsAdmin, Guid id);
    Task<CatalogueEntry?> UpdateEntryAsync(Guid userAccountId, bool isAdmin, Guid id, uint? publishedYear, Genre? genre, uint? mark);
    Task<CatalogueEntry> CreateEntryAsync(Guid userAccountId, string? title, string? author, uint? publishedYear, Genre? genre, uint? mark);
    Task<CatalogueEntry?> DeleteEntryAsync(Guid userAccountId, bool isAdmin, Guid id);
}