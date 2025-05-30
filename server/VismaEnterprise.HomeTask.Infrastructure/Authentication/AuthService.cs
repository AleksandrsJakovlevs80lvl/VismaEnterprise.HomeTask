using Microsoft.Extensions.Options;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.DTOs;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Options;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication;

public class AuthService(IUserAccountService userAccountService, IPasswordManager passwordManager, ITokenManager tokenManager, IOptions<JwtOptions> jwtOptions) : IAuthService
{
    public async Task<RegisterResultDto> RegisterAsync(string? username, string? password)
    {
        var userAccount = await userAccountService.Create(username, passwordManager.Encrypt(password), false);

        return new RegisterResultDto
        {
            Success = true,
            Username = userAccount.Username
        };
    }

    public async Task<LoginResultDto> LoginAsync(string? username, string? password)
    {
        var userAccount = await userAccountService.Find(username);

        if (!passwordManager.Verify(password, userAccount.Password))
        {
            throw new InvalidOperationException("Password is incorrect");
        }

        var tokenInfo = tokenManager.GetToken(userAccount, jwtOptions.Value);

        return new LoginResultDto
        {
            Success = true,
            Token = tokenInfo.Token,
            Expires = tokenInfo.Expires
        };
    }
}