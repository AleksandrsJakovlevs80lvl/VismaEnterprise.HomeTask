using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VismaEnterprise.HomeTask.Domain.Entities;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Options;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication;

public class TokenManager : ITokenManager
{
    public TokenOptions GetToken(UserAccount userAccount, JwtOptions jwtOptions)
    {
        var key = Encoding.ASCII.GetBytes(jwtOptions.Key);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(GetClaims(userAccount)),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new TokenOptions
        {
            Token = tokenString,
            Expires = token.ValidTo
        };
    }

    private static Claim[] GetClaims(UserAccount userAccount)
    {
        return [
            new Claim(ClaimTypes.NameIdentifier, userAccount.PublicId.ToString()),
            new Claim(ClaimTypes.Name, userAccount.Username),
            new Claim(ClaimTypes.Role, userAccount.IsAdmin.ToString())
        ];
    }
}