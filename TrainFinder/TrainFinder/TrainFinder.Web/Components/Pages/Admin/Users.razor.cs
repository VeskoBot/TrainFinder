using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrainFinder.Application.Interfaces;
using UserEntity = TrainFinder.Data.Entities.User;

namespace TrainFinder.Web.Components.Pages.Admin;

public partial class Users
{
    private static readonly Guid AdminRoleId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
    private static readonly Guid UserRoleId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private List<UserEntity> _users = new();
    private bool _isLoading = true;
    private Guid? _currentUserId;

    private string? _searchTerm;
    private string? SearchTerm
    {
        get => _searchTerm;
        set
        {
            _searchTerm = value;
            CurrentPage = 1;
        }
    }

    private int CurrentPage { get; set; } = 1;
    private const int PageSize = 10;

    private IEnumerable<UserEntity> FilteredUsers
    {
        get
        {
            var results = _users.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLowerInvariant();
                results = results.Where(u =>
                    u.FullName.ToLowerInvariant().Contains(term) ||
                    u.Email.ToLowerInvariant().Contains(term) ||
                    GetRoleDisplay(u).ToLowerInvariant().Contains(term));
            }
            return results;
        }
    }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredUsers.Count() / (double)PageSize));

    private IEnumerable<UserEntity> PaginatedUsers =>
        FilteredUsers.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
    private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var oid = authState.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                      ?? authState.User.FindFirst("oid")?.Value;

            if (string.IsNullOrEmpty(oid))
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            var currentUser = await UserService.GetByExternalObjectIdAsync(oid, CancellationToken.None);
            if (currentUser == null || currentUser.Role.Name != "Admin")
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _currentUserId = currentUser.Id;

            var users = await UserService.GetAllAsync(CancellationToken.None);
            _users = users.ToList();
        }
        catch { }
        _isLoading = false;
    }

    private bool IsSelf(UserEntity user) => _currentUserId.HasValue && user.Id == _currentUserId.Value;

    private bool IsAdmin(UserEntity user) => user.RoleId == AdminRoleId;

    private string GetRoleDisplay(UserEntity user) =>
        user.RoleId == AdminRoleId ? "Администратор" : "Потребител";

    private async Task ToggleAdmin(UserEntity user, ChangeEventArgs e)
    {
        var isAdmin = (bool)(e.Value ?? false);
        user.RoleId = isAdmin ? AdminRoleId : UserRoleId;
        await UserService.UpdateUserAsync(user, CancellationToken.None);
    }

    private async Task DeleteUser(UserEntity user)
    {
        await UserService.DeleteUserAsync(user.Id, CancellationToken.None);
        _users.Remove(user);
    }
}
