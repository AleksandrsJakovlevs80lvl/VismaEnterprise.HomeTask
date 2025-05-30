using System.Security.Claims;
using VismaEnterprise.HomeTask.Application.Data;
using VismaEnterprise.HomeTask.Application.Services.Interfaces;

namespace VismaEnterprise.HomeTask.Web.Services;

public class UserContextResolver(IHttpContextAccessor httpContextAccessor) : IUserContextResolver
{
    public UserContext? Resolve()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        var isAdminClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (isAdminClaim == null || !bool.TryParse(isAdminClaim, out var isAdmin))
        {
            return null;
        }

        return new UserContext
        {
            UserAccountId = userId,
            IsAdmin = isAdmin
        };
    }
}