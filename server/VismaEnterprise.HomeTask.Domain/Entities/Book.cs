using JetBrains.Annotations;
using VismaEnterprise.HomeTask.Domain.ValueObjects;

namespace VismaEnterprise.HomeTask.Domain.Entities;

public class Book : IEquatable<Book>
{
    public Guid PublicId { get; init; }
    public string Title { get; init; }
    public Author Author { get; init; }
    public uint? PublishedYear { get; private set; }
    public Genre? Genre { get; private set; }

    private Book(Guid id, string title, Author author, uint? publishedYear, Genre? genre)
    {
        PublicId = id;
        Title = title;
        Author = author;
        PublishedYear = publishedYear;
        Genre = genre;
    }

    public static Book Create(string? title, Author? author, uint? publishedYear, Genre? genre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(author);

        if (publishedYear > DateTime.Today.Year)
        {
            throw new ArgumentException("Published year cannot be in the future.", nameof(publishedYear));
        }

        return new Book(Guid.NewGuid(), title, author, publishedYear, genre);
    }

    public void Update(uint? publishedYear, Genre? genre)
    {
        if (publishedYear > DateTime.Today.Year)
        {
            throw new ArgumentException("Published year cannot be in the future.", nameof(publishedYear));
        }

        PublishedYear = publishedYear;
        Genre = genre;
    }

    #region [Entity Framework]

    [UsedImplicitly]
    private Book() {}

    [UsedImplicitly]
    public int Id { get; private set; }

    #endregion

    #region [Equality Members]

    public bool Equals(Book? other)
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
        return Equals((Book)obj);
    }

    public override int GetHashCode()
    {
        return PublicId.GetHashCode();
    }

    #endregion
}

public enum Genre
{
    Fiction,
    NonFiction,
    Mystery,
    Fantasy,
    ScienceFiction,
    Biography,
    History,
    Romance,
    Thriller,
    Horror
}