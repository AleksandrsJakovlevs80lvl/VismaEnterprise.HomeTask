using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Repositories;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByUsernameAsync(string? username);
    Task AddAsync(UserAccount userAccount);
}