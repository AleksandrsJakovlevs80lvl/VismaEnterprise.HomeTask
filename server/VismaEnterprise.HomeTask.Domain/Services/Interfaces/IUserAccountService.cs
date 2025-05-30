using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Services.Interfaces;

public interface IUserAccountService
{
    Task<UserAccount> Find(string? username);
    Task<UserAccount> Create(string? username, string? password, bool? isAdmin);
}