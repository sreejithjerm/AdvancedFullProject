using Asp.Versioning;
using AdvancedFullProject.Application.Auth.Commands;
using AdvancedFullProject.Application.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedFullProject.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterCommand command, ISender sender, CancellationToken cancellationToken)
        => Ok(await sender.Send(command, cancellationToken));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, ISender sender, CancellationToken cancellationToken)
        => Ok(await sender.Send(command, cancellationToken));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenCommand command, ISender sender, CancellationToken cancellationToken)
        => Ok(await sender.Send(command, cancellationToken));
}
