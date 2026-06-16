using TrainFinder.Data.Entities;

namespace TrainFinder.Repository.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Request>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Request request, CancellationToken cancellationToken);
    void Update(Request request);
    void Delete(Request request);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
