using Microsoft.Extensions.Logging;
using TrainFinder.Application.Interfaces;
using TrainFinder.Application.Parsers.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Application.Services;

public class TrainImportService : ITrainImportService
{
    private readonly ITrainService _trainService;
    private readonly IStationService _stationService;
    private readonly ITrainLocationService _trainLocationService;
    private readonly ITimetableService _timetableService;
    private readonly ITimetableImportService _timetableImportService;
    private readonly IBdzRadarClient _bdzRadarClient;
    private readonly IBdzRadarTrainParser _parser;
    private readonly ILogger<TrainImportService> _logger;

    public TrainImportService(
        ITrainService trainService,
        IStationService stationService,
        ITrainLocationService trainLocationService,
        ITimetableService timetableService,
        ITimetableImportService timetableImportService,
        IBdzRadarClient bdzRadarClient,
        IBdzRadarTrainParser parser,
        ILogger<TrainImportService> logger)
    {
        _trainService = trainService;
        _stationService = stationService;
        _trainLocationService = trainLocationService;
        _timetableService = timetableService;
        _timetableImportService = timetableImportService;
        _bdzRadarClient = bdzRadarClient;
        _parser = parser;
        _logger = logger;
    }

    public async Task RefreshTrainDataAsync(CancellationToken cancellationToken = default)
    {
        var html = await _bdzRadarClient.GetRadarHtmlAsync(cancellationToken);

        var parsedTrains = _parser.Parse(html);

        Console.WriteLine($"Parsed trains: {parsedTrains.Count}");

        var trainsWithoutTimetable = new List<Train>();

        foreach (var parsedTrain in parsedTrains)
        {
            var train = await _trainService.GetOrCreateTrainAsync(
                trainNumber: parsedTrain.TrainNumber,
                category: (TrainCategory)parsedTrain.CategoryId,
                wagonCount: parsedTrain.WagonCount,
                cancellationToken);

            Station? station = null;

            if (!string.IsNullOrWhiteSpace(parsedTrain.StationCode))
            {
                station = await _stationService.GetOrCreateStationAsync(
                    code: parsedTrain.StationCode,
                    name: parsedTrain.StationName ?? parsedTrain.StationCode,
                    cancellationToken);
            }

            Station? nextStation = null;

            if (!string.IsNullOrWhiteSpace(parsedTrain.NextStationCode))
            {
                if (parsedTrain.NextStationCode == parsedTrain.StationCode)
                {
                    nextStation = station;
                }
                else
                {
                    nextStation = await _stationService.GetOrCreateStationAsync(
                        code: parsedTrain.NextStationCode,
                        name: parsedTrain.NextStationName ?? parsedTrain.NextStationCode,
                        cancellationToken);
                }
            }

            await _trainLocationService.UpdateLocationAsync(
                trainId: train.Id,
                stationId: station?.Id,
                nextStationId: nextStation?.Id,
                latitude: parsedTrain.Latitude,
                longitude: parsedTrain.Longitude,
                delayMinutes: parsedTrain.DelayMinutes,
                timePlanned: parsedTrain.TimePlanned,
                reportedAt: parsedTrain.ReportedAt,
                cancellationToken);

            var existingTimetable = await _timetableService.GetByTrainIdAsync(train.Id, cancellationToken);

            if (existingTimetable is null)
            {
                trainsWithoutTimetable.Add(train);
            }
        }

        foreach (var train in trainsWithoutTimetable)
        {
            try
            {
                await _timetableImportService.ImportTimetableForTrainAsync(train, cancellationToken);
                _logger.LogInformation("Imported timetable for new train {TrainNumber}.", train.TrainNumber);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to import timetable for new train {TrainNumber}.", train.TrainNumber);
            }
        }
    }
}