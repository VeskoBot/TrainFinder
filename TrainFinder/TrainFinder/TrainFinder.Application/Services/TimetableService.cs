using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class TimetableService : ITimetableService
{
    private readonly ITimetableRepository _timetableRepository;

    public TimetableService(ITimetableRepository timetableRepository)
    {
        _timetableRepository = timetableRepository;
    }

    public async Task<Timetable?> GetByTrainIdAsync(Guid trainId, CancellationToken cancellationToken)
    {
        return await _timetableRepository.GetByTrainIdAsync(trainId, cancellationToken);
    }

    public async Task AddAsync(Timetable timetable, CancellationToken cancellationToken)
    {
        await _timetableRepository.AddAsync(timetable, cancellationToken);
    }

    public async Task UpdateAsync(Timetable timetable, CancellationToken cancellationToken)
    {
        _timetableRepository.Update(timetable);
    }

    public Task DeleteStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken)
    {
        _timetableRepository.DeleteStops(stops);
        return Task.CompletedTask;
    }

    public async Task AddStopsAsync(IEnumerable<TimetableStop> stops, CancellationToken cancellationToken)
    {
        await _timetableRepository.AddStopsAsync(stops, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _timetableRepository.SaveChangesAsync(cancellationToken);
    }
}
