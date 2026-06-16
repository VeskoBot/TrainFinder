using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class StationService : IStationService
{
    private readonly IStationRepository _stationRepository;

    public StationService(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    public async Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _stationRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _stationRepository.GetAllAsync(cancellationToken);
    }

    public async Task AddAsync(Station station, CancellationToken cancellationToken)
    {
        await _stationRepository.AddAsync(station, cancellationToken);
        await _stationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Station station, CancellationToken cancellationToken)
    {
        _stationRepository.Update(station);
        await _stationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Station station, CancellationToken cancellationToken)
    {
        _stationRepository.Delete(station);
        await _stationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _stationRepository.GetByCodeAsync(code, cancellationToken);
    }

    public async Task<Station> GetOrCreateStationAsync(
        string code,
        string name,
        CancellationToken cancellationToken)
    {
        var station = await _stationRepository.GetOrCreateAsync(
            code, name, cancellationToken);

        await _stationRepository.SaveChangesAsync(cancellationToken);

        return station;
    }

    public async Task<Station> GetOrCreateByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _stationRepository.GetOrCreateByNameAsync(
            name, cancellationToken);
    }
}
