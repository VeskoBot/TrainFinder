namespace TrainFinder.Data.Entities;

public class User
{
    public Guid Id { get; set; }

    public string ExternalObjectId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? MobileNumber { get; set; }

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
