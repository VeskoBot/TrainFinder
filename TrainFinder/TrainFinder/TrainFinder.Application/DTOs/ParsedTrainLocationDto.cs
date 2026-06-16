namespace TrainFinder.Application.Dtos;

public class ParsedTrainLocationDto
{
    public int TrainNumber { get; set; }

    public int CategoryId { get; set; }

    public int WagonCount { get; set; }

    public string? StationCode { get; set; }

    public string? StationName { get; set; }

    public string? NextStationCode { get; set; }

    public string? NextStationName { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public int DelayMinutes { get; set; }

    public DateTime? TimePlanned { get; set; }

    public DateTime ReportedAt { get; set; }
}