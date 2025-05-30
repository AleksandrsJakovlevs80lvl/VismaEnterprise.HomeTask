using VismaEnterprise.HomeTask.Application.DTOs;
using VismaEnterprise.HomeTask.Domain.Aggregates;
using VismaEnterprise.HomeTask.Domain.Entities;

namespace VismaEnterprise.HomeTask.Application.Mappers.Interfaces;

public interface ICatalogueEntryMapper
{
    CatalogueEntryDto Map(CatalogueEntry catalogueEntry);
    Genre? MapGenre(string? genreString);
}