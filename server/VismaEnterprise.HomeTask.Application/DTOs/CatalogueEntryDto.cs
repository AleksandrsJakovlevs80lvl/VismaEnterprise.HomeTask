namespace VismaEnterprise.HomeTask.Application.DTOs;

public class CatalogueEntryDto
{
    public Guid? PublicId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public uint? PublishedYear { get; set; }
    public string? Genre { get; set;  }
    public uint? Mark { get; set; }
}