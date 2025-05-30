using VismaEnterprise.HomeTask.Infrastructure.Authentication.DTOs;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;

public interface IAuthService
{
    Task<RegisterResultDto> RegisterAsync(string? userName, string? password);
    Task<LoginResultDto> LoginAsync(string? userName, string? password);
}