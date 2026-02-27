using AdvancedFullProject.Application.Auth.DTOs;
using AdvancedFullProject.Domain.Entities;

namespace AdvancedFullProject.Application.Abstractions.Security;

public interface IJwtTokenService
{
    AuthResponse GenerateTokens(User user);
}
