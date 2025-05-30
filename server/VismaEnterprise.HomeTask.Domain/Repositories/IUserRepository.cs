using VismaEnterprise.HomeTask.Domain.Aggregates;

namespace VismaEnterprise.HomeTask.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUserAccountIdAsync(Guid userAccountId);
    Task AddAsync(User user);
}