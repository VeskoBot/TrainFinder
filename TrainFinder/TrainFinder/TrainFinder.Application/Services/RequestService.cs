using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;
using TrainFinder.Application.Interfaces;
using TrainFinder.Application.Helpers;
using TrainFinder.Repository.Interfaces;

namespace TrainFinder.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _requestRepository;

    public RequestService(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _requestRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _requestRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Request>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _requestRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<Request> CreateAsync(Guid userId, string title, string content, CancellationToken cancellationToken)
    {
        var request = new Request
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            Status = RequestStatus.Draft,
            UserId = userId,
            CreatedAt = TimeHelper.GetEasternEuropeanTime(),
            UpdatedAt = TimeHelper.GetEasternEuropeanTime()
        };

        await _requestRepository.AddAsync(request, cancellationToken);
        await _requestRepository.SaveChangesAsync(cancellationToken);

        return request;
    }

    public async Task UpdateAsync(Request request, CancellationToken cancellationToken)
    {
        request.UpdatedAt = TimeHelper.GetEasternEuropeanTime();
        _requestRepository.Update(request);
        await _requestRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Request not found.");

        _requestRepository.Delete(request);
        await _requestRepository.SaveChangesAsync(cancellationToken);
    }
}
