using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using TrainFinder.Application.Interfaces;

namespace TrainFinder.Web.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject]
        private IGraphUserService GraphUserService { get; set; } = default!;

        [Inject]
        private IServiceScopeFactory ScopeFactory { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private string? _displayName;
        private string? _email;
        private string? _mobilePhone;
        private string? _mobilePhoneError;
        private bool _loading = true;
        private bool _saving;
        private bool _saved;
        private string? _error;

        private void OnMobilePhoneInput(ChangeEventArgs e)
        {
            _mobilePhone = e.Value?.ToString();
            ValidateMobilePhone();
        }

        private void ValidateMobilePhone()
        {
            if (!string.IsNullOrEmpty(_mobilePhone) && !System.Text.RegularExpressions.Regex.IsMatch(_mobilePhone, @"^[0-9]+$"))
                _mobilePhoneError = "Телефонният номер трябва да съдържа само цифри.";
            else
                _mobilePhoneError = null;
        }

        private string? _externalObjectId;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                _externalObjectId = authState.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                                    ?? authState.User.FindFirst("oid")?.Value;

                var profile = await GraphUserService.GetUserProfileAsync();

                if (profile != null)
                {
                    _displayName = profile.DisplayName;
                    _email = profile.Email;
                    _mobilePhone = profile.MobileNumber;

                    if (!string.IsNullOrEmpty(_externalObjectId))
                    {
                        await using var scope = ScopeFactory.CreateAsyncScope();
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        var dbUser = await userService.GetByExternalObjectIdAsync(_externalObjectId, CancellationToken.None);
                        if (dbUser != null)
                        {
                            dbUser.FullName = profile.DisplayName ?? dbUser.FullName;
                            dbUser.Email = profile.Email ?? dbUser.Email;
                            dbUser.MobileNumber = profile.MobileNumber;
                            await userService.UpdateUserAsync(dbUser, CancellationToken.None);
                        }
                    }
                }
            }
            catch
            {
                _error = "Грешка при зареждане на профила.";
            }
            finally
            {
                _loading = false;
            }
        }

        private async Task SaveProfile()
        {
            _saving = true;
            _saved = false;
            _error = null;

            if (string.IsNullOrEmpty(_externalObjectId))
            {
                _error = "Не е намерен идентификатор на потребителя.";
                _saving = false;
                return;
            }

            ValidateMobilePhone();
            if (_mobilePhoneError != null)
            {
                _saving = false;
                return;
            }

            try
            {
                await GraphUserService.UpdateUserProfileAsync(_externalObjectId, _displayName, null, null, _mobilePhone);

                await using var scope = ScopeFactory.CreateAsyncScope();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var dbUser = await userService.GetByExternalObjectIdAsync(_externalObjectId, CancellationToken.None);
                if (dbUser != null)
                {
                    dbUser.FullName = _displayName ?? dbUser.FullName;
                    dbUser.MobileNumber = _mobilePhone;
                    await userService.UpdateUserAsync(dbUser, CancellationToken.None);
                }

                _saved = true;
            }
            catch (Exception)
            {
                _error = "Грешка при запазване на профила. Моля, опитайте отново.";
            }
            finally
            {
                _saving = false;
            }
        }
    }
}
