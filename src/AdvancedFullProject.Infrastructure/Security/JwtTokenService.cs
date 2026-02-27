using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdvancedFullProject.Application.Abstractions.Security;
using AdvancedFullProject.Application.Auth.DTOs;
using AdvancedFullProject.Domain.Entities;
using AdvancedFullProject.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdvancedFullProject.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    public JwtTokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public AuthResponse GenerateTokens(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role)],
            expires: expires,
            signingCredentials: creds);

        return new AuthResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            ExpiresAt = expires
        };
    }
}
