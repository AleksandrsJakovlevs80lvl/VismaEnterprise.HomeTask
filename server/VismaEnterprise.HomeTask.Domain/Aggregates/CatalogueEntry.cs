using JetBrains.Annotations;
using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Domain.Aggregates;

public class CatalogueEntry : IEquatable<CatalogueEntry>
{
    public Guid PublicId { get; init; }
    public User User { get; init; }
    public Book Book { get; init; }
    public uint? Mark { get; private set; }

    private CatalogueEntry(Guid id, User user, Book book, uint? mark)
    {
        PublicId = id;
        User = user;
        Book = book;
        Mark = mark;
    }

    public static CatalogueEntry Create(User? user, Book? book, uint? mark)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(book);

        if (mark is < 1 or > 5)
        {
            throw new ArgumentException("Mark must be between 1 and 5.", nameof(mark));
        }

        return new CatalogueEntry(Guid.NewGuid(), user, book, mark);
    }

    public void Update(uint? mark)
    {
        if (mark is < 1 or > 5)
        {
            throw new ArgumentException("Mark must be between 1 and 5.", nameof(mark));
        }

        Mark = mark;
    }

    #region [Entity Framework]

    [UsedImplicitly]
    private CatalogueEntry() {}

    [UsedImplicitly]
    public int Id { get; private set; }

    [UsedImplicitly]
    public int UserId { get; private set; }

    [UsedImplicitly]
    public int BookId { get; private set; }

    #endregion

    #region [Equality Members]

    public bool Equals(CatalogueEntry? other)
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
        return Equals((CatalogueEntry)obj);
    }

    public override int GetHashCode()
    {
        return PublicId.GetHashCode();
    }

    #endregion
}