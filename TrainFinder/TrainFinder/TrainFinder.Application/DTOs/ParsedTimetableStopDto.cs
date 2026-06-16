namespace TrainFinder.Application.Dtos;

public class ParsedTimetableStopDto
{
    public int StopOrder { get; set; }

    public string StationName { get; set; } = string.Empty;

    public TimeOnly? ArrivalTime { get; set; }

    public TimeOnly? DepartureTime { get; set; }
}
