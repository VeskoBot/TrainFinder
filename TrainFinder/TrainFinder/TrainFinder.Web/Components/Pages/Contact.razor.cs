using Microsoft.AspNetCore.Components;
using TrainFinder.Application.Interfaces;

namespace TrainFinder.Web.Components.Pages;

public partial class Contact : ComponentBase
{
    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private IEmailService EmailService { get; set; } = default!;

    private string? _email;
    private string? _title;
    private string? _description;
    private string? _emailError;
    private bool _saving;
    private bool _submitted;

    private void OnEmailInput(ChangeEventArgs e)
    {
        _email = e.Value?.ToString();
        ValidateEmail();
    }

    private void ValidateEmail()
    {
        if (string.IsNullOrWhiteSpace(_email))
        {
            _emailError = "Имейл адресът е задължителен.";
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(_email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            _emailError = "Въведете валиден имейл адрес.";
        }
        else
        {
            _emailError = null;
        }
    }

    private string? _sendError;

    private async Task SubmitAsync()
    {
        ValidateEmail();

        if (_emailError != null || string.IsNullOrWhiteSpace(_title) || string.IsNullOrWhiteSpace(_description))
            return;

        _saving = true;
        _sendError = null;

        try
        {
            var allUsers = await UserService.GetAllAsync(CancellationToken.None);
            var adminEmails = allUsers
                .Where(u => u.Role.Name == "Admin" && u.IsEnabled)
                .Select(u => u.Email)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToList();

            if (adminEmails.Count > 0)
            {
                var subject = $"TrainFinder - {_title}";
                var body = $"Запитване от: {_email}\n\n{_description}";
                await EmailService.SendAsync(adminEmails, subject, body, replyTo: _email);
            }
        }
        catch (Exception ex)
        {
            _sendError = $"Грешка при изпращане";
            _saving = false;
            return;
        }

        _submitted = true;
        _saving = false;
    }
}
