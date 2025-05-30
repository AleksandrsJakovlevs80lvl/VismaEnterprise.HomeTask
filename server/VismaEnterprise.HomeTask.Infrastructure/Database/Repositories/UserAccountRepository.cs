using Microsoft.EntityFrameworkCore;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Domain.Repositories;

namespace VismaEnterprise.HomeTask.Infrastructure.Database.Repositories;

public class UserAccountRepository(HomeTaskDbContext context) : IUserAccountRepository
{
    public async Task<UserAccount?> GetByUsernameAsync(string? username)
    {
        if (username == null)
        {
            return null;
        }

        return await context.UserAccounts.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(UserAccount userAccount)
    {
        await context.UserAccounts.AddAsync(userAccount);
        await context.SaveChangesAsync();
    }
}