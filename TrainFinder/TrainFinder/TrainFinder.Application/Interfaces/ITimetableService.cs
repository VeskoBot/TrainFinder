using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Interfaces;

public interface ITimetableService
{
    Task<Timetable?> GetByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task AddAsync(Timetable timetable, CancellationToken cancellationToken);

    Task UpdateAsync(Timetable timetable, CancellationToken cancellationToken);

    Task DeleteStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken);

    Task AddStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
