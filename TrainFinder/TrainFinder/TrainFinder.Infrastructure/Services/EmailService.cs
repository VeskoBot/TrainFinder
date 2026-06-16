using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TrainFinder.Application.Interfaces;

namespace TrainFinder.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(IEnumerable<string> recipients, string subject, string body, string? replyTo = null, CancellationToken cancellationToken = default)
    {
        var smtp = _configuration.GetSection("Smtp");
        var host = smtp["Host"] ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.Parse(smtp["Port"] ?? "587");
        var fromAddress = smtp["From"] ?? throw new InvalidOperationException("Smtp:From is not configured.");
        var username = smtp["Username"] ?? fromAddress;
        var password = smtp["Password"] ?? throw new InvalidOperationException("Smtp:Password is not configured.");
        var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(fromAddress, "TrainFinder"),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        if (!string.IsNullOrWhiteSpace(replyTo))
            message.ReplyToList.Add(new MailAddress(replyTo));

        foreach (var recipient in recipients)
            message.To.Add(recipient);

        if (message.To.Count == 0)
            return;

        await client.SendMailAsync(message, cancellationToken);
    }
}
