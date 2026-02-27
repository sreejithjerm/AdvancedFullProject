using AdvancedFullProject.Application.Auth.DTOs;
using MediatR;

namespace AdvancedFullProject.Application.Auth.Commands;

public sealed record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;
