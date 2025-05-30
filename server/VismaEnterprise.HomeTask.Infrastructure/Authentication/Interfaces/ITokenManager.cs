using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Options;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;

public interface ITokenManager
{
    TokenOptions GetToken(UserAccount userAccount, JwtOptions jwtOptions);
}

public class TokenOptions
{
    public required string Token { get; init; }
    public required DateTime Expires { get; init; }
}