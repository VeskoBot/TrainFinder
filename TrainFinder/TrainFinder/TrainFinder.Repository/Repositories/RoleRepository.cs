using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Context;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Repository.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly TrainFinderDbContext _context;

    public RoleRepository(TrainFinderDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Roles.ToListAsync(cancellationToken);
    }
}
