using Microsoft.Extensions.Logging;
using TrainFinder.Application.Helpers;
using TrainFinder.Application.Interfaces;
using TrainFinder.Application.Parsers.Interfaces;
using TrainFinder.Data.Entities;

namespace TrainFinder.Application.Services;

public class TimetableImportService : ITimetableImportService
{
    private readonly ITrainService _trainService;
    private readonly ITimetableService _timetableService;
    private readonly IStationService _stationService;
    private readonly IBdzTimetableClient _timetableClient;
    private readonly IBdzTimetableParser _timetableParser;
    private readonly ILogger<TimetableImportService> _logger;

    public TimetableImportService(
        ITrainService trainService,
        ITimetableService timetableService,
        IStationService stationService,
        IBdzTimetableClient timetableClient,
        IBdzTimetableParser timetableParser,
        ILogger<TimetableImportService> logger)
    {
        _trainService = trainService;
        _timetableService = timetableService;
        _stationService = stationService;
        _timetableClient = timetableClient;
        _timetableParser = timetableParser;
        _logger = logger;
    }

    public async Task ImportAllTimetablesAsync(CancellationToken cancellationToken)
    {
        var trains = await _trainService.GetAllAsync(cancellationToken);

        foreach (var train in trains)
        {
            try
            {
                await ImportTimetableForTrainAsync(train, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import timetable for train {TrainNumber}.", train.TrainNumber);
            }
        }
    }

    public async Task ImportTimetableForTrainAsync(Train train, CancellationToken cancellationToken)
    {
        var html = await _timetableClient.GetTimetableHtmlAsync(train.TrainNumber, cancellationToken);
        var parsedStops = _timetableParser.Parse(html);

        if (parsedStops.Count == 0)
        {
            _logger.LogWarning("No timetable stops found for train {TrainNumber}.", train.TrainNumber);
            return;
        }

        var existingTimetable = await _timetableService.GetByTrainIdAsync(train.Id, cancellationToken);
        var now = TimeHelper.GetEasternEuropeanTime();

        var stops = new List<TimetableStop>();

        foreach (var s in parsedStops)
        {
            var station = await _stationService.GetOrCreateByNameAsync(s.StationName, cancellationToken);

            stops.Add(new TimetableStop
            {
                Id = Guid.NewGuid(),
                StopOrder = s.StopOrder,
                StationId = station.Id,
                ArrivalTime = s.ArrivalTime,
                DepartureTime = s.DepartureTime
            });
        }

        if (existingTimetable != null)
        {
            await _timetableService.DeleteStopsAsync(existingTimetable.Stops, cancellationToken);

            foreach (var stop in stops)
            {
                stop.TimetableId = existingTimetable.Id;
            }

            existingTimetable.UpdatedAt = now;
            await _timetableService.AddStopsAsync(stops, cancellationToken);
            await _timetableService.UpdateAsync(existingTimetable, cancellationToken);
        }
        else
        {
            var timetable = new Timetable
            {
                Id = Guid.NewGuid(),
                TrainId = train.Id,
                CreatedAt = now,
                UpdatedAt = now,
            };

            foreach (var stop in stops)
            {
                stop.TimetableId = timetable.Id;
            }

            await _timetableService.AddAsync(timetable, cancellationToken);
            await _timetableService.AddStopsAsync(stops, cancellationToken);
        }

        await _timetableService.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Imported timetable for train {TrainNumber} with {StopCount} stops.",
            train.TrainNumber,
            parsedStops.Count);
    }
}
