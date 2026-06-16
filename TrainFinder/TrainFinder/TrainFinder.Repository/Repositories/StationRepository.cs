using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class StationRepository : IStationRepository
{
    private readonly TrainFinderDbContext _context;

    public StationRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Stations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Stations.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Station station, CancellationToken cancellationToken)
    {
        await _context.Stations.AddAsync(station, cancellationToken);
    }

    public void Update(Station station)
    {
        _context.Stations.Update(station);
    }

    public void Delete(Station station)
    {
        _context.Stations.Remove(station);
    }

    public async Task<Station?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        return await _context.Stations
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<Station> GetOrCreateAsync(
        string code,
        string name,
        CancellationToken cancellationToken)
    {
        var station = _context.Stations.Local
            .FirstOrDefault(x => x.Code == code);

        if (station is null)
        {
            station = await _context.Stations
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }

        var now = DateTime.UtcNow;

        if (station is null)
        {
            station = new Station
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Stations.AddAsync(station, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(name) && station.Name != name)
        {
            station.Name = name;
            station.UpdatedAt = now;
        }

        return station;
    }

    public async Task<Station> GetOrCreateByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        var station = _context.Stations.Local
            .FirstOrDefault(x => x.Name == name);

        if (station is null)
        {
            station = await _context.Stations
                .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        if (station is null)
        {
            var now = DateTime.UtcNow;

            station = new Station
            {
                Id = Guid.NewGuid(),
                Code = string.Empty,
                Name = name,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Stations.AddAsync(station, cancellationToken);
        }

        return station;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
