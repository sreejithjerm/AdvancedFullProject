using AdvancedFullProject.Application.Abstractions.Persistence;
using AdvancedFullProject.Application.Abstractions.Security;
using AdvancedFullProject.Application.Auth.Commands;
using AdvancedFullProject.Application.Auth.DTOs;
using AdvancedFullProject.Domain.Entities;
using MediatR;

namespace AdvancedFullProject.Application.Auth;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User { Username = request.Username, Email = request.Email, PasswordHash = _hasher.Hash(request.Password), Role = request.Role };
        return await _users.CreateAsync(user, cancellationToken);
    }
}

public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;

    public LoginHandler(IUserRepository users, IPasswordHasher hasher, IJwtTokenService tokens, IRefreshTokenRepository refreshTokens)
    {
        _users = users; _hasher = hasher; _tokens = tokens; _refreshTokens = refreshTokens;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByUsernameAsync(request.Username, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid credentials.");
        if (!_hasher.Verify(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials.");
        var response = _tokens.GenerateTokens(user);
        await _refreshTokens.SaveAsync(new RefreshToken { UserId = user.Id, Token = response.RefreshToken, ExpiresAt = response.ExpiresAt.AddDays(7) }, cancellationToken);
        return response;
    }
}

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _tokens;

    public RefreshTokenHandler(IRefreshTokenRepository refreshTokens, IUserRepository users, IJwtTokenService tokens)
    {
        _refreshTokens = refreshTokens; _users = users; _tokens = tokens;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await _refreshTokens.GetAsync(request.RefreshToken, cancellationToken);
        if (existing is null || existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = await _users.GetByIdAsync(existing.UserId, cancellationToken) ?? throw new UnauthorizedAccessException("User not found");
        await _refreshTokens.RevokeAsync(request.RefreshToken, cancellationToken);

        var response = _tokens.GenerateTokens(user);
        await _refreshTokens.SaveAsync(new RefreshToken { UserId = user.Id, Token = response.RefreshToken, ExpiresAt = response.ExpiresAt.AddDays(7) }, cancellationToken);
        return response;
    }
}
