using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Application.Interfaces;

public interface ITrainService
{
    Task<Train?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Train>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Train train, CancellationToken cancellationToken);

    Task UpdateAsync(Train train, CancellationToken cancellationToken);

    Task DeleteAsync(Train train, CancellationToken cancellationToken);

    Task<Train> GetOrCreateTrainAsync(
        int trainNumber,
        TrainCategory category,
        int wagonCount,
        CancellationToken cancellationToken);
}
