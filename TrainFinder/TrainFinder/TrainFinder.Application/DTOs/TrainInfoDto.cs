using TrainFinder.Data.Enums;

namespace TrainFinder.Application.DTOs
{
    public class TrainInfoDto
    {
        public int TrainNumber { get; set; }
        public TrainCategory Category { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DelayMinutes { get; set; }
        public int WagonCount { get; set; }
        public string? StationName { get; set; }
        public string? NextStationName { get; set; }
        public string? StartStationName { get; set; }
        public string? FinalStationName { get; set; }
        public DateTime? LastReportedAt { get; set; }
        public int TotalStops { get; set; }
        public int PassedStops { get; set; }
        public int ProgressPercent => TotalStops > 0 ? (int)Math.Round(PassedStops * 100.0 / TotalStops) : 0;
        public string? CurrentStationArrivalTime { get; set; }
        public string? NextStationDepartureTime { get; set; }
        public List<TimetableStopInfoDto> TimetableStops { get; set; } = new();
    }

    public class TimetableStopInfoDto
    {
        public int StopOrder { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string? ArrivalTime { get; set; }
        public string? DepartureTime { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsPassed { get; set; }
    }
}
