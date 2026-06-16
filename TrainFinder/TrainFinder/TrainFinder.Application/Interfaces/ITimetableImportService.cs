using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Interfaces;

public interface ITimetableImportService
{
    Task ImportAllTimetablesAsync(CancellationToken cancellationToken);

    Task ImportTimetableForTrainAsync(Train train, CancellationToken cancellationToken);
}
