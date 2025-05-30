using VismaEnterprise.HomeTask.Application.Data;

namespace VismaEnterprise.HomeTask.Application.Services.Interfaces;

public interface IUserContextResolver
{
    UserContext? Resolve();
}