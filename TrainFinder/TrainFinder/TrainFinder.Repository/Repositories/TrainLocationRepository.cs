using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class TrainLocationRepository : ITrainLocationRepository
{
    private readonly TrainFinderDbContext _context;

    public TrainLocationRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<TrainCurrentLocation?> GetCurrentLocationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.TrainCurrentLocations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TrainCurrentLocation?> GetCurrentLocationByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _context.TrainCurrentLocations
            .FirstOrDefaultAsync(x => x.TrainId == trainId, cancellationToken);
    }

    public async Task<IEnumerable<TrainCurrentLocation>> GetAllCurrentLocationsAsync(CancellationToken cancellationToken)
    {
        return await _context.TrainCurrentLocations.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TrainCurrentLocation>> GetCurrentLocationsUpdatedSinceAsync(DateTime since, CancellationToken cancellationToken)
    {
        return await _context.TrainCurrentLocations
            .Include(x => x.Station)
            .Include(x => x.NextStation)
            .Include(x => x.Train)
            .Where(x => x.UpdatedAt >= since)
            .ToListAsync(cancellationToken);
    }

    public async Task AddCurrentLocationAsync(TrainCurrentLocation location, CancellationToken cancellationToken)
    {
        await _context.TrainCurrentLocations.AddAsync(location, cancellationToken);
    }

    public void UpdateCurrentLocation(TrainCurrentLocation location)
    {
        _context.TrainCurrentLocations.Update(location);
    }

    public void DeleteCurrentLocation(TrainCurrentLocation location)
    {
        _context.TrainCurrentLocations.Remove(location);
    }

    public async Task<TrainLocationHistory?> GetHistoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.TrainLocationHistory
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TrainLocationHistory>> GetHistoryByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _context.TrainLocationHistory
            .Where(x => x.TrainId == trainId)
            .OrderByDescending(x => x.ReportedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddHistoryAsync(TrainLocationHistory history, CancellationToken cancellationToken)
    {
        await _context.TrainLocationHistory.AddAsync(history, cancellationToken);
    }

    public void DeleteHistory(TrainLocationHistory history)
    {
        _context.TrainLocationHistory.Remove(history);
    }

    public async Task UpsertCurrentLocationAsync(
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
        var currentLocation = await _context.TrainCurrentLocations
            .FirstOrDefaultAsync(x => x.TrainId == trainId, cancellationToken);

        if (currentLocation is null)
        {
            currentLocation = new TrainCurrentLocation
            {
                Id = Guid.NewGuid(),
                TrainId = trainId
            };

            await _context.TrainCurrentLocations.AddAsync(currentLocation, cancellationToken);
        }

        currentLocation.StationId = stationId;
        currentLocation.NextStationId = nextStationId;
        currentLocation.Latitude = latitude;
        currentLocation.Longitude = longitude;
        currentLocation.DelayMinutes = delayMinutes;
        currentLocation.TimePlanned = timePlanned;
        currentLocation.LastReportedAt = reportedAt;
        currentLocation.UpdatedAt = DateTime.UtcNow;
    }

    public async Task AddHistoryAsync(
        Guid trainId,
        Guid? stationId,
        Guid? nextStationId,
        double latitude,
        double longitude,
        int delayMinutes,
        DateTime reportedAt,
        CancellationToken cancellationToken)
    {
        var history = new TrainLocationHistory
        {
            Id = Guid.NewGuid(),
            TrainId = trainId,
            StationId = stationId,
            NextStationId = nextStationId,
            Latitude = latitude,
            Longitude = longitude,
            DelayMinutes = delayMinutes,
            ReportedAt = reportedAt,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TrainLocationHistory.AddAsync(history, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
