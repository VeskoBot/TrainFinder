namespace TrainFinder.Data.Entities;

public class TrainCurrentLocation
{
    public Guid Id { get; set; }

    public Guid TrainId { get; set; }

    public Train Train { get; set; } = default!;

    public Guid? StationId { get; set; }

    public Station? Station { get; set; }

    public Guid? NextStationId { get; set; }

    public Station? NextStation { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public int DelayMinutes { get; set; }

    public DateTime? TimePlanned { get; set; }

    public DateTime LastReportedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}