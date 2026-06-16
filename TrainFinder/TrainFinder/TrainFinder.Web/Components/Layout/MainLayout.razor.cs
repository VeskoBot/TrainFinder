using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrainFinder.Application.Interfaces;

namespace TrainFinder.Web.Components.Layout;

public partial class MainLayout
{
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    private bool _isAuthenticated;
    private string _userRole = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            _isAuthenticated = authState.User.Identity?.IsAuthenticated == true;

            if (_isAuthenticated)
            {
                var oid = authState.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                          ?? authState.User.FindFirst("oid")?.Value;

                if (!string.IsNullOrEmpty(oid))
                {
                    var user = await UserService.GetByExternalObjectIdAsync(oid, CancellationToken.None);
                    _userRole = user?.Role?.Name ?? "User";
                }
            }
        }
        catch
        {

        }
    }
}
