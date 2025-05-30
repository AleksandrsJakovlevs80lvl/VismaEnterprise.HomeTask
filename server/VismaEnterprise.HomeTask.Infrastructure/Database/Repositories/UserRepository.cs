using Microsoft.EntityFrameworkCore;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Repositories;

namespace VismaEnterprise.HomeTask.Infrastructure.Database.Repositories;

public class UserRepository(HomeTaskDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<User?> GetByUserAccountIdAsync(Guid userAccountId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserAccount.PublicId == userAccountId);
    }
}