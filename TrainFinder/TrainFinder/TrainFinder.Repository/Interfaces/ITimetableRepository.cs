using TrainFinder.Data.Entities;

namespace TrainFinder.Repository.Interfaces;

public interface ITimetableRepository
{
    Task<Timetable?> GetByTrainIdAsync(Guid trainId, CancellationToken cancellationToken);

    Task AddAsync(Timetable timetable, CancellationToken cancellationToken);

    void Update(Timetable timetable);

    void DeleteStops(IEnumerable<TimetableStop> stops);

    Task AddStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
