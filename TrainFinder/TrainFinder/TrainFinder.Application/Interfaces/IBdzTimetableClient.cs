namespace TrainFinder.Application.Interfaces;

public interface IBdzTimetableClient
{
    Task<string> GetTimetableHtmlAsync(int trainNumber, CancellationToken cancellationToken = default);
}
