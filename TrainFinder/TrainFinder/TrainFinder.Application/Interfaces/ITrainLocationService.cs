using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Interfaces;

public interface ITrainLocationService
{
    Task<TrainCurrentLocation?> GetCurrentLocationByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task<IEnumerable<TrainCurrentLocation>> GetAllCurrentLocationsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<TrainCurrentLocation>> GetRecentCurrentLocationsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<TrainLocationHistory>> GetHistoryByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task DeleteCurrentLocationAsync(TrainCurrentLocation location, CancellationToken cancellationToken);

    Task DeleteHistoryAsync(TrainLocationHistory history, CancellationToken cancellationToken);

    Task UpdateLocationAsync(
        Guid trainId,
        Guid? stationId,
        Guid? nextStationId,
        double latitude,
        double longitude,
        int delayMinutes,
        DateTime? timePlanned,
        DateTime reportedAt,
        CancellationToken cancellationToken);
}
