using AdvancedFullProject.Application.Common.Models;
using AdvancedFullProject.Domain.Entities;

namespace AdvancedFullProject.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Employee>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(Employee entity, CancellationToken cancellationToken);
    Task UpdateAsync(Employee entity, CancellationToken cancellationToken);
    Task SoftDeleteAsync(Guid id, string user, CancellationToken cancellationToken);
}
