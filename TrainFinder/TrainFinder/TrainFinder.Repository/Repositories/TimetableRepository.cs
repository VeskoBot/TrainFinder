using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class TimetableRepository : ITimetableRepository
{
    private readonly TrainFinderDbContext _context;

    public TimetableRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<Timetable?> GetByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _context.Timetables
            .Include(t => t.Stops.OrderBy(s => s.StopOrder))
                .ThenInclude(s => s.Station)
            .FirstOrDefaultAsync(t => t.TrainId == trainId, cancellationToken);
    }

    public async Task AddAsync(Timetable timetable, CancellationToken cancellationToken)
    {
        timetable.Stops = null;
        await _context.Timetables.AddAsync(timetable, cancellationToken);
    }

    public void Update(Timetable timetable)
    {
        _context.Entry(timetable).State = EntityState.Modified;
    }

    public void DeleteStops(IEnumerable<TimetableStop> stops)
    {
        _context.TimetableStops.RemoveRange(stops);
    }

    public async Task AddStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken)
    {
        foreach (var stop in stops)
        {
            stop.Station = null;
        }
        await _context.TimetableStops.AddRangeAsync(stops, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
