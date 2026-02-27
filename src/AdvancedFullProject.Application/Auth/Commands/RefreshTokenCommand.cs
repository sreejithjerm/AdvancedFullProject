using AdvancedFullProject.Application.Auth.DTOs;
using MediatR;

namespace AdvancedFullProject.Application.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
