using Microsoft.AspNetCore.Mvc;
using VismaEnterprise.HomeTask.Infrastructure.Authentication.Interfaces;
using VismaEnterprise.HomeTask.Web.DTOs;

namespace VismaEnterprise.HomeTask.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCredentialsDto userCredentials)
    {
        var result = await authService.RegisterAsync(userCredentials.Username, userCredentials.Password);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserCredentialsDto userCredentials)
    {
        var result =  await authService.LoginAsync(userCredentials.Username, userCredentials.Password);
        return Ok(result);
    }
}