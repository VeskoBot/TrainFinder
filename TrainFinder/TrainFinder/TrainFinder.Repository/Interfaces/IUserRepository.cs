using TrainFinder.Data.Entities;

namespace TrainFinder.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByExternalObjectIdAsync(string externalObjectId, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Update(User user);
    void Delete(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
