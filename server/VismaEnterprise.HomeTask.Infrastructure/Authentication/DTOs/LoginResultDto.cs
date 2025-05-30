namespace VismaEnterprise.HomeTask.Infrastructure.Authentication.DTOs;

public class LoginResultDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? Expires { get; set; }
}