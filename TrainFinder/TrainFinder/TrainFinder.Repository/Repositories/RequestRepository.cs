using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly TrainFinderDbContext _context;

    public RequestRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Requests
            .Include(r => r.User)
            .Include(r => r.UpdatedBy)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Requests
            .Include(r => r.User)
            .Include(r => r.UpdatedBy)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Requests
            .Include(r => r.UpdatedBy)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Request request, CancellationToken cancellationToken)
    {
        await _context.Requests.AddAsync(request, cancellationToken);
    }

    public void Update(Request request)
    {
        _context.Requests.Update(request);
    }

    public void Delete(Request request)
    {
        _context.Requests.Remove(request);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
