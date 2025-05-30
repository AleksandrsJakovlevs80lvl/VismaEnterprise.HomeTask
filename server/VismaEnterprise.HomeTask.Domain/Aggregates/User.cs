using JetBrains.Annotations;
using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Aggregates;

public class User : IEquatable<User>
{
    public Guid PublicId { get; init; }
    public string Name { get; init; }
    public UserAccount UserAccount { get; init; }

    private User(Guid id, string name, UserAccount userAccount)
    {
        PublicId = id;
        Name = name;
        UserAccount = userAccount;
    }

    public static User Create(string? name, UserAccount userAccount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new User(Guid.NewGuid(), name, userAccount);
    }

    #region [Entity Framework]

    [UsedImplicitly]
    private User() {}

    [UsedImplicitly]
    public int Id { get; private set; }

    [UsedImplicitly]
    public int UserAccountId { get; private set; }

    #endregion

    #region [Equality Members]

    public bool Equals(User? other)
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
        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return PublicId.GetHashCode();
    }

    #endregion
}