namespace TrainFinder.Application.Interfaces;

public interface IBdzRadarClient
{
    Task<string> GetRadarHtmlAsync(CancellationToken cancellationToken = default);
}