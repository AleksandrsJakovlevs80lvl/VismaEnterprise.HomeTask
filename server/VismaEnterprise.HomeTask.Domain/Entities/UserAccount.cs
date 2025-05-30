using JetBrains.Annotations;

namespace VismaEnterprise.HomeTask.Domain.Entities;

public class UserAccount : IEquatable<UserAccount>
{
    public Guid PublicId { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public bool IsAdmin { get; init; }

    private UserAccount(Guid id, string username, string password, bool isAdmin)
    {
        PublicId = id;
        Username = username;
        Password = password;
        IsAdmin = isAdmin;
    }

    public static UserAccount Create(string? username, string? password, bool? isAdmin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        if (!isAdmin.HasValue)
        {
            throw new ArgumentNullException(nameof(isAdmin));
        }

        return new UserAccount(Guid.NewGuid(), username, password, isAdmin.Value);
    }

    #region [Entity Framework]

    [UsedImplicitly]
    private UserAccount() {}

    [UsedImplicitly]
    public int Id { get; private set; }

    #endregion

    #region [Equality Members]

    public bool Equals(UserAccount? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return PublicId.Equals(other.PublicId);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UserAccount)obj);
    }

    public override int GetHashCode()
    {
        return PublicId.GetHashCode();
    }

    #endregion
}