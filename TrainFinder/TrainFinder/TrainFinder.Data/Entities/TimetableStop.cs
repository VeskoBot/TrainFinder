namespace TrainFinder.Data.Entities;

public class TimetableStop
{
    public Guid Id { get; set; }

    public Guid TimetableId { get; set; }

    public Timetable Timetable { get; set; } = default!;

    public int StopOrder { get; set; }

    public Guid StationId { get; set; }

    public Station Station { get; set; } = default!;

    public TimeOnly? ArrivalTime { get; set; }

    public TimeOnly? DepartureTime { get; set; }
}
