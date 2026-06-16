using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class TrainService : ITrainService
{
    private readonly ITrainRepository _trainRepository;

    public TrainService(ITrainRepository trainRepository)
    {
        _trainRepository = trainRepository;
    }

    public async Task<Train?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _trainRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Train>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _trainRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(Train train, CancellationToken cancellationToken)
    {
        await _trainRepository.AddAsync(train, cancellationToken);
        await _trainRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Train train, CancellationToken cancellationToken)
    {
        _trainRepository.Update(train);
        await _trainRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Train train, CancellationToken cancellationToken)
    {
        _trainRepository.Delete(train);
        await _trainRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<Train> GetOrCreateTrainAsync(
        int trainNumber,
        TrainCategory category,
        int wagonCount,
        CancellationToken cancellationToken)
    {
        var train = await _trainRepository.GetOrCreateAsync(
            trainNumber, category, wagonCount, cancellationToken);

        await _trainRepository.SaveChangesAsync(cancellationToken);

        return train;
    }
}
