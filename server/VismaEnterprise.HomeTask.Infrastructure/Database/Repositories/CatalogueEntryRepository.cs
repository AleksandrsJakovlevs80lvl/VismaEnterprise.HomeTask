using Microsoft.EntityFrameworkCore;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Repositories;

namespace VismaEnterprise.HomeTask.Infrastructure.Database.Repositories;

public class CatalogueEntryRepository(HomeTaskDbContext context) : ICatalogueEntryRepository
{
    public async Task<List<CatalogueEntry>> GetAllAsync()
    {
        return await context.CatalogueEntries
            .Include(ce => ce.Book)
            .ToListAsync();
    }

    public async Task<List<CatalogueEntry>> GetAllByUserAsync(Guid userId)
    {
        return await context.CatalogueEntries
            .Where(e => e.User.UserAccount.PublicId == userId)
            .Include(ce => ce.Book)
            .ToListAsync();
    }

    public async Task<CatalogueEntry?> GetByIdAsync(Guid id)
    {
        return await context.CatalogueEntries
            .Include(ce => ce.Book)
            .Include(ce => ce.User)
            .Include(ce => ce.User.UserAccount)
            .FirstOrDefaultAsync(e => e.PublicId == id);
    }

    public async Task UpdateAsync(CatalogueEntry entry)
    {
        context.CatalogueEntries.Update(entry);
        await context.SaveChangesAsync();
    }

    public async Task AddAsync(CatalogueEntry entry)
    {
        await context.CatalogueEntries.AddAsync(entry);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CatalogueEntry entry)
    {
        context.CatalogueEntries.Remove(entry);
        await context.SaveChangesAsync();
    }
}