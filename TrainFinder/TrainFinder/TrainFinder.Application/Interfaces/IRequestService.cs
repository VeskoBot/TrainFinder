using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Application.Interfaces;

public interface IRequestService
{
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Request>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Request> CreateAsync(Guid userId, string title, string content, CancellationToken cancellationToken);
    Task UpdateAsync(Request request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
