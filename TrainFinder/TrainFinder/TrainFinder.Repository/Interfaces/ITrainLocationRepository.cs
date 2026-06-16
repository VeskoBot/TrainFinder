using TrainFinder.Data.Entities;

namespace TrainFinder.Repository.Interfaces;

public interface ITrainLocationRepository
{
    Task<TrainCurrentLocation?> GetCurrentLocationByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TrainCurrentLocation?> GetCurrentLocationByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task<IEnumerable<TrainCurrentLocation>> GetAllCurrentLocationsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<TrainCurrentLocation>> GetCurrentLocationsUpdatedSinceAsync(DateTime since, CancellationToken cancellationToken);

    Task AddCurrentLocationAsync(TrainCurrentLocation location, CancellationToken cancellationToken);

    void UpdateCurrentLocation(TrainCurrentLocation location);

    void DeleteCurrentLocation(TrainCurrentLocation location);

    Task<TrainLocationHistory?> GetHistoryByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<TrainLocationHistory>> GetHistoryByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task AddHistoryAsync(TrainLocationHistory history, CancellationToken cancellationToken);

    void DeleteHistory(TrainLocationHistory history);

    Task UpsertCurrentLocationAsync(
        Guid trainId,
        Guid? stationId,
        Guid? nextStationId,
        double latitude,
        double longitude,
        int delayMinutes,
        DateTime? timePlanned,
        DateTime reportedAt,
        CancellationToken cancellationToken);

    Task AddHistoryAsync(
        Guid trainId,
        Guid? stationId,
        Guid? nextStationId,
        double latitude,
        double longitude,
        int delayMinutes,
        DateTime reportedAt,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
