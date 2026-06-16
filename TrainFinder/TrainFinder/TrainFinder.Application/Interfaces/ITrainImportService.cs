namespace TrainFinder.Application.Interfaces;

public interface ITrainImportService
{
    Task RefreshTrainDataAsync(CancellationToken cancellationToken = default);
}