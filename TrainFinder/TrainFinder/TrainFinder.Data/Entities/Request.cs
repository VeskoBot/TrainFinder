using TrainFinder.Data.Enums;

namespace TrainFinder.Data.Entities;

public class Request
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? Response { get; set; }

    public RequestStatus Status { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid? UpdatedById { get; set; }

    public User? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
