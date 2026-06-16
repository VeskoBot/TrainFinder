using TrainFinder.Data.Enums;

namespace TrainFinder.Data.Entities;

public class Train
{
    public Guid Id { get; set; }

    public int TrainNumber { get; set; }

    public TrainCategory Category { get; set; }

    public int WagonCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public TrainCurrentLocation? CurrentLocation { get; set; }

    public List<TrainLocationHistory> LocationHistory { get; set; } = new();
}
