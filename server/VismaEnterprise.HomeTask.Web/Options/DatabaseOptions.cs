using System.ComponentModel.DataAnnotations;

namespace VismaEnterprise.HomeTask.Web.Options;

public class DatabaseOptions
{
    [Required]
    public string? DefaultConnection { get; init; }
}