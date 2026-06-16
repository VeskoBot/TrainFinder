using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Repository.Interfaces;

public interface ITrainRepository
{
    Task<Train?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Train>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Train train, CancellationToken cancellationToken);

    void Update(Train train);

    void Delete(Train train);

    Task<Train> GetOrCreateAsync(
        int trainNumber,
        TrainCategory category,
        int wagonCount,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
