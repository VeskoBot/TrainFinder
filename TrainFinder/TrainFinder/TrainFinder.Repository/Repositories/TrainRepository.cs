using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class TrainRepository : ITrainRepository
{
    private readonly TrainFinderDbContext _context;

    public TrainRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<Train?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Trains
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Train>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Trains.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Train train, CancellationToken cancellationToken)
    {
        await _context.Trains.AddAsync(train, cancellationToken);
    }

    public void Update(Train train)
    {
        _context.Trains.Update(train);
    }

    public void Delete(Train train)
    {
        _context.Trains.Remove(train);
    }

    public async Task<Train> GetOrCreateAsync(
        int trainNumber,
        TrainCategory category,
        int wagonCount,
        CancellationToken cancellationToken)
    {
        var train = await _context.Trains
            .FirstOrDefaultAsync(x => x.TrainNumber == trainNumber, cancellationToken);

        var now = DateTime.UtcNow;

        if (train is null)
        {
            train = new Train
            {
                Id = Guid.NewGuid(),
                TrainNumber = trainNumber,
                Category = category,
                WagonCount = wagonCount,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _context.Trains.AddAsync(train, cancellationToken);
        }
        else
        {
            train.Category = category;
            train.WagonCount = wagonCount;
            train.UpdatedAt = now;
        }

        return train;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
