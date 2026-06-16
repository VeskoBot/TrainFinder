using TrainFinder.Application.Helpers;
using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IGraphUserService _graphUserService;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IGraphUserService graphUserService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _graphUserService = graphUserService;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User?> GetByExternalObjectIdAsync(string externalObjectId, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByExternalObjectIdAsync(externalObjectId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }

    public async Task<User> CreateUserAsync(string externalObjectId, string fullName, string email, string? mobileNumber, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByNameAsync("User", cancellationToken)
            ?? throw new InvalidOperationException("Default 'User' role not found.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            ExternalObjectId = externalObjectId,
            FullName = fullName,
            Email = email,
            MobileNumber = mobileNumber,
            RoleId = role.Id,
            IsEnabled = true,
            CreatedAt = TimeHelper.GetEasternEuropeanTime(),
            UpdatedAt = TimeHelper.GetEasternEuropeanTime()
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        user.UpdatedAt = TimeHelper.GetEasternEuropeanTime();
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        try
        {
            await _graphUserService.DeleteUserAsync(user.ExternalObjectId);
        }
        catch (Exception)
        {
            var rollbackUser = new User
            {
                Id = user.Id,
                ExternalObjectId = user.ExternalObjectId,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                RoleId = user.RoleId,
                IsEnabled = user.IsEnabled,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            await _userRepository.AddAsync(rollbackUser, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException(
                "Failed to delete user from external identity provider. Database deletion has been rolled back.", 
                innerException: null);
        }
    }
}
