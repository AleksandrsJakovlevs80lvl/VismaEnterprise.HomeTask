using System.ComponentModel.DataAnnotations;

namespace VismaEnterprise.HomeTask.Web.Options;

public class AdminOptions
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}