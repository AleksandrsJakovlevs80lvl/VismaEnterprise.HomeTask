using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Repositories;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;

namespace VismaEnterprise.HomeTask.Domain.Services;

public class UserAccountService(IUserAccountRepository repository, IUserRepository userRepository) : IUserAccountService
{
    public async Task<UserAccount> Find(string? username)
    {
        var existingUser = await repository.GetByUsernameAsync(username);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with {username} does not exist");
        }

        return existingUser;
    }

    public async Task<UserAccount> Create(string? username, string? password, bool? isAdmin)
    {
        var userAccount = UserAccount.Create(username, password, isAdmin);
        var user = User.Create(username, userAccount);

        var existingUser = await repository.GetByUsernameAsync(userAccount.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with {username} already exists");
        }

        await repository.AddAsync(userAccount);
        await userRepository.AddAsync(user);

        return userAccount;
    }
}