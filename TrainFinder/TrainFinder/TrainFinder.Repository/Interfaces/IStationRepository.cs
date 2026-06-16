using TrainFinder.Data.Entities;

namespace TrainFinder.Repository.Interfaces;

public interface IStationRepository
{
    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Station station, CancellationToken cancellationToken);

    void Update(Station station);

    void Delete(Station station);

    Task<Station?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken);

    Task<Station> GetOrCreateAsync(
        string code,
        string name,
        CancellationToken cancellationToken);

    Task<Station> GetOrCreateByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
