namespace VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;

public interface IPasswordManager
{
    string Encrypt(string? password);
    bool Verify(string? password, string? encryptedPassword);
}