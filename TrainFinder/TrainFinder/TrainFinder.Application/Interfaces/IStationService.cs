using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Interfaces;

public interface IStationService
{
    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Station station, CancellationToken cancellationToken);

    Task UpdateAsync(Station station, CancellationToken cancellationToken);

    Task DeleteAsync(Station station, CancellationToken cancellationToken);

    Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    Task<Station> GetOrCreateStationAsync(
        string code,
        string name,
        CancellationToken cancellationToken);

    Task<Station> GetOrCreateByNameAsync(
        string name,
        CancellationToken cancellationToken);
}
