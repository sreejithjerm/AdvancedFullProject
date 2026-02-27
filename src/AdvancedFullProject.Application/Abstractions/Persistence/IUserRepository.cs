using AdvancedFullProject.Domain.Entities;

namespace AdvancedFullProject.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
}
