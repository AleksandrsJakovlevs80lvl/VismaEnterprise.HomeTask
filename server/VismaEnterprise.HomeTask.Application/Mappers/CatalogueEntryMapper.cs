using VismaEnterprise.HomeTask.Application.DTOs;
using VismaEnterprise.HomeTask.Application.Mappers.Interfaces;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Application.Mappers;

public class CatalogueEntryMapper : ICatalogueEntryMapper
{
    public CatalogueEntryDto Map(CatalogueEntry catalogueEntry)
    {
        return new CatalogueEntryDto
        {
            PublicId = catalogueEntry.PublicId,
            Title = catalogueEntry.Book.Title,
            Author = catalogueEntry.Book.Author.Name + " " + catalogueEntry.Book.Author.Surname,
            PublishedYear = catalogueEntry.Book.PublishedYear,
            Genre = catalogueEntry.Book.Genre?.ToString(),
            Mark = catalogueEntry.Mark
        };
    }

    public Genre? MapGenre(string? genreString)
    {
        if (string.IsNullOrWhiteSpace(genreString))
        {
            return null;
        }

        if (!Enum.TryParse(genreString, out Genre genre))
        {
            throw new InvalidOperationException("Can not resolve genre.");
        }

        return genre;
    }
}