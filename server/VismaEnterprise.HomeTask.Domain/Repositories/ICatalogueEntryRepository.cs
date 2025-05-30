using VismaEnterprise.HomeTask.Domain.Aggregates;

namespace VismaEnterprise.HomeTask.Domain.Repositories;

public interface ICatalogueEntryRepository
{
    Task<List<CatalogueEntry>> GetAllAsync();
    Task<List<CatalogueEntry>> GetAllByUserAsync(Guid userId);
    Task<CatalogueEntry?> GetByIdAsync(Guid id);
    Task UpdateAsync(CatalogueEntry entry);
    Task AddAsync(CatalogueEntry entry);
    Task DeleteAsync(CatalogueEntry entry);
}