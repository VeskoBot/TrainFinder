namespace TrainFinder.Data.Entities;

public class Timetable
{
    public Guid Id { get; set; }

    public Guid TrainId { get; set; }

    public Train Train { get; set; } = default!;

    public List<TimetableStop> Stops { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
