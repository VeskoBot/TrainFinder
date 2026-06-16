using TrainFinder.Application.Helpers;
using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class TrainLocationService : ITrainLocationService
{
    private readonly ITrainLocationRepository _trainLocationRepository;

    public TrainLocationService(ITrainLocationRepository trainLocationRepository)
    {
        _trainLocationRepository = trainLocationRepository;
    }

    public async Task<TrainCurrentLocation?> GetCurrentLocationByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _trainLocationRepository.GetCurrentLocationByTrainIdAsync(trainId, cancellationToken);
    }

    public async Task<IEnumerable<TrainCurrentLocation>> GetAllCurrentLocationsAsync(CancellationToken cancellationToken)
    {
        return await _trainLocationRepository.GetAllCurrentLocationsAsync(cancellationToken);
    }

    public async Task<IEnumerable<TrainCurrentLocation>> GetRecentCurrentLocationsAsync(CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddMinutes(-10);
        return await _trainLocationRepository.GetCurrentLocationsUpdatedSinceAsync(since, cancellationToken);
    }

    public async Task<IEnumerable<TrainLocationHistory>> GetHistoryByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _trainLocationRepository.GetHistoryByTrainIdAsync(trainId, cancellationToken);
    }

    public async Task DeleteCurrentLocationAsync(TrainCurrentLocation location, CancellationToken cancellationToken)
    {
        _trainLocationRepository.DeleteCurrentLocation(location);
        await _trainLocationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteHistoryAsync(TrainLocationHistory history, CancellationToken cancellationToken)
    {
        _trainLocationRepository.DeleteHistory(history);
        await _trainLocationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateLocationAsync(
        Guid trainId,
        Guid? stationId,
        Guid? nextStationId,
        double latitude,
        double longitude,
        int delayMinutes,
        DateTime? timePlanned,
        DateTime reportedAt,
        CancellationToken cancellationToken)
    {
        await _trainLocationRepository.UpsertCurrentLocationAsync(
            trainId, stationId, nextStationId,
            latitude, longitude, delayMinutes,
            timePlanned, reportedAt, cancellationToken);

        await _trainLocationRepository.AddHistoryAsync(
            trainId, stationId, nextStationId,
            latitude, longitude, delayMinutes,
            reportedAt, cancellationToken);

        await _trainLocationRepository.SaveChangesAsync(cancellationToken);
    }
}
