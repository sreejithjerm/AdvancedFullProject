using AdvancedFullProject.Domain.Entities;

namespace AdvancedFullProject.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task SaveAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken);
    Task RevokeAsync(string token, CancellationToken cancellationToken);
}
