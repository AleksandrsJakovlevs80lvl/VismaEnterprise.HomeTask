using Microsoft.Extensions.Options;
using VismaEnterprise.HomeTask.Domain.Services.Interfaces;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;
using VismaEnterprise.HomeTask.Web.Options;

namespace VismaEnterprise.HomeTask.Web.Services;

public class EnsureAdminUserCreated(IServiceProvider serviceProvider, IOptions<AdminOptions> adminOptions) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var passwordManager = scope.ServiceProvider.GetRequiredService<IPasswordManager>();
            var userAccountService = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
            await userAccountService.Create(adminOptions.Value.Username, passwordManager.Encrypt(adminOptions.Value.Password), true);
        }
        catch
        {
            // ignored
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}