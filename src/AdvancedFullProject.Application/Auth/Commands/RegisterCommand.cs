using MediatR;

namespace AdvancedFullProject.Application.Auth.Commands;

public sealed record RegisterCommand(string Username, string Email, string Password, string Role) : IRequest<Guid>;
