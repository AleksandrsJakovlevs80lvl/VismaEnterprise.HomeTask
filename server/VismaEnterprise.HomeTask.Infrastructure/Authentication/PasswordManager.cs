using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication;

public class PasswordManager : IPasswordManager
{
    public string Encrypt(string? password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string? password, string? encryptedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, encryptedPassword);
    }
}