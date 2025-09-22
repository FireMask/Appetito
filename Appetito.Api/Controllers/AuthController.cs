using Microsoft.AspNetCore.Mvc;

namespace Appetito.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var ua = Request.Headers.UserAgent.ToString();
        var res = await auth.LoginAsync(dto.Email, dto.Password, ip, ua, ct);
        return Ok(res);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var ua = Request.Headers.UserAgent.ToString();
        var res = await auth.RefreshAsync(dto.RefreshToken, ip, ua, ct);
        return Ok(res);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        var ua = Request.Headers.UserAgent.ToString();
        var res = await auth.RegisterAsync(dto.Email, dto.Password, ip, ua, ct);
        return Ok(res);
    }

    public sealed record LoginDto(string Email, string Password);
    public sealed record RefreshDto(string RefreshToken);
    public sealed record RegisterDto(string Email, string Password);
}