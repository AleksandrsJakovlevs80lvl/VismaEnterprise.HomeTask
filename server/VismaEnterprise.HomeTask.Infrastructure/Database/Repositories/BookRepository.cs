using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Repositories;

namespace VismaEnterprise.HomeTask.Infrastructure.Database.Repositories;

public class BookRepository(HomeTaskDbContext context) : IBookRepository
{
    public async Task AddAsync(Book newBook)
    {
        await context.Books.AddAsync(newBook);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book existingBook)
    {
        context.Books.Update(existingBook);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book existingBook)
    {
        context.Books.Remove(existingBook);
        await context.SaveChangesAsync();
    }
}