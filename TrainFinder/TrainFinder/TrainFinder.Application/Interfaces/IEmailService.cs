namespace TrainFinder.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(IEnumerable<string> recipients, string subject, string body, string? replyTo = null, CancellationToken cancellationToken = default);
}
