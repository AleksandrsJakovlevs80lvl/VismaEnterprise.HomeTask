using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Repositories;

public interface IBookRepository
{
    Task AddAsync(Book newBook);
    Task UpdateAsync(Book existingBook);
    Task DeleteAsync(Book entryBook);
}