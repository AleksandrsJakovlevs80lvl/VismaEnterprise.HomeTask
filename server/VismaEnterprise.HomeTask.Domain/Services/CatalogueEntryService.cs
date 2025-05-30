using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Repositories;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;
using VismaEnterprise.HomeTask.Domain.ValueObjects;

namespace VismaEnterprise.HomeTask.Domain.Services;

public class CatalogueEntryService(ICatalogueEntryRepository repository, IBookRepository bookRepository, IUserRepository userRepository) : ICatalogueEntryService
{
    public async Task<List<CatalogueEntry>> GetEntriesAsync(Guid userAccountId, bool isAdmin)
    {
        if (isAdmin)
        {
            return await repository.GetAllAsync();
        }
        else
        {
            return await repository.GetAllByUserAsync(userAccountId);
        }
    }

    public async Task<CatalogueEntry?> GetEntryAsync(Guid userAccountId, bool isAdmin, Guid id)
    {
        var entry = await repository.GetByIdAsync(id);
        if (entry == null)
        {
            return null;
        }

        if (!isAdmin && entry.User.UserAccount.PublicId != userAccountId)
        {
            throw new UnauthorizedAccessException("You do not have permission to access this catalogue entry.");
        }

        return entry;
    }

    public async Task<CatalogueEntry?> UpdateEntryAsync(Guid userAccountId, bool isAdmin, Guid id, uint? publishedYear, Genre? genre, uint? mark)
    {
        var entry = await GetEntryAsync(userAccountId, isAdmin, id);
        if (entry == null)
        {
            return null;
        }

        entry.Book.Update(publishedYear, genre);
        entry.Update(mark);

        await bookRepository.UpdateAsync(entry.Book);
        await repository.UpdateAsync(entry);

        return entry;
    }

    public async Task<CatalogueEntry> CreateEntryAsync(Guid userAccountId, string? title, string? author, uint? publishedYear, Genre? genre, uint? mark)
    {
        var user = await userRepository.GetByUserAccountIdAsync(userAccountId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var book = Book.Create(title, Author.Create(author), publishedYear, genre);
        var entry = CatalogueEntry.Create(user, book, mark);

        await bookRepository.AddAsync(book);
        await repository.AddAsync(entry);

        return entry;
    }

    public async Task<CatalogueEntry?> DeleteEntryAsync(Guid userAccountId, bool isAdmin, Guid id)
    {
        var entry = await GetEntryAsync(userAccountId, isAdmin, id);
        if (entry == null)
        {
            return null;
        }

        await repository.DeleteAsync(entry);
        await bookRepository.DeleteAsync(entry.Book);

        return entry;
    }
}