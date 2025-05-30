using System.ComponentModel.DataAnnotations;

namespace VismaEnterprise.HomeTask.Domain.ValueObjects;

public class Author : IEquatable<Author>
{
    [Required]
    public string Name { get; }

    [Required]
    public string Surname { get; }

    private Author(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    public static Author Create(string? author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        var authorNameSplit = author.Split(' ');
        if (authorNameSplit.Length != 2)
        {
            throw new ArgumentException("Author name should have format \"{Name} {Surname}\"", nameof(author));
        }

        return new Author(authorNameSplit[0], authorNameSplit[1]);
    }

    #region [Equality Members]

    public bool Equals(Author? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Surname == other.Surname;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Author)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname);
    }

    #endregion
}