using System.ComponentModel.DataAnnotations;

namespace VismaEnterprise.HomeTask.Infrastructure.Authentication.Options;

public class JwtOptions
{
    [Required]
    public string? Key { get; init; }

    [Required]
    public string? Issuer { get; init; }

    [Required]
    public string? Audience { get; init; }
}