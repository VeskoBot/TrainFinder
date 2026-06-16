using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Interfaces;

public interface IRoleService
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken);
}
