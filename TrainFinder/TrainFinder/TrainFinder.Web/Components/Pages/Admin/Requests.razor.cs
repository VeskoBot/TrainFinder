using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Web.Components.Pages.Admin;

public partial class Requests
{
    [Inject]
    private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IUserService UserService { get; set; } = default!;

    [Inject]
    private IRequestService RequestService { get; set; } = default!;

    private List<Request> RequestList { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private Guid? SelectedRequestId { get; set; }
    private Request? SelectedRequest => RequestList.FirstOrDefault(r => r.Id == SelectedRequestId);

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

    private bool FilterPending { get; set; } = true;
    private bool FilterClosed { get; set; } = true;

    private int CurrentPage { get; set; } = 1;
    private const int PageSize = 10;

    private int PendingCount => FilteredBySearch.Count(r => r.Status == RequestStatus.Pending);
    private int ClosedCount => FilteredBySearch.Count(r => r.Status == RequestStatus.Closed);

    private string? ResponseText { get; set; }
    private bool _isSaving;
    private bool _showConfirmDialog;
    private string _confirmMessage = string.Empty;
    private Guid? _currentUserId;

    private IEnumerable<Request> FilteredBySearch
    {
        get
        {
            var results = RequestList.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLowerInvariant();
                results = results.Where(r =>
                    r.Title.ToLowerInvariant().Contains(term) ||
                    r.Content.ToLowerInvariant().Contains(term) ||
                    r.User.FullName.ToLowerInvariant().Contains(term) ||
                    r.User.Email.ToLowerInvariant().Contains(term));
            }
            return results;
        }
    }

    private IEnumerable<Request> FilteredRequests
    {
        get
        {
            return FilteredBySearch.Where(r =>
                (FilterPending && r.Status == RequestStatus.Pending) ||
                (FilterClosed && r.Status == RequestStatus.Closed));
        }
    }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredRequests.Count() / (double)PageSize));

    private IEnumerable<Request> PaginatedRequests =>
        FilteredRequests.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private void TogglePending() { FilterPending = !FilterPending; CurrentPage = 1; }
    private void ToggleClosed() { FilterClosed = !FilterClosed; CurrentPage = 1; }
    private void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
    private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }

    private void SelectRequest(Request request)
    {
        SelectedRequestId = request.Id;
        ResponseText = request.Status == RequestStatus.Pending ? "" : null;
    }

    private void RespondToRequest()
    {
        if (string.IsNullOrWhiteSpace(ResponseText))
        {
            _confirmMessage = "Сигурни ли сте, че искате да затворите заявката без отговор?";
        }
        else
        {
            _confirmMessage = "Сигурни ли сте, че искате да изпратите отговора и да затворите заявката?";
        }
        _showConfirmDialog = true;
    }

    private void CancelRespond()
    {
        _showConfirmDialog = false;
    }

    private async Task ConfirmRespond()
    {
        _showConfirmDialog = false;

        if (SelectedRequest == null || _isSaving) return;

        _isSaving = true;
        try
        {
            SelectedRequest.Response = string.IsNullOrWhiteSpace(ResponseText) ? null : ResponseText;
            SelectedRequest.Status = RequestStatus.Closed;
            SelectedRequest.UpdatedById = _currentUserId;
            await RequestService.UpdateAsync(SelectedRequest, CancellationToken.None);

            var index = RequestList.FindIndex(r => r.Id == SelectedRequest.Id);
            if (index >= 0)
            {
                RequestList[index] = SelectedRequest;
            }

            ResponseText = null;
        }
        finally
        {
            _isSaving = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

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

            var allRequests = await RequestService.GetAllAsync(CancellationToken.None);
            RequestList = allRequests.Where(r => r.Status != RequestStatus.Draft).ToList();
        }
        catch { }

        IsLoading = false;
    }

    private string GetToggleClass(bool active) => $"filter-toggle {(active ? "active" : "")}";

    private string GetStatusDisplay(RequestStatus status) => status switch
    {
        RequestStatus.Pending => "Изпратена",
        RequestStatus.Closed => "Затворена",
        _ => "Неизвестен"
    };

    private string GetStatusBadgeClass(RequestStatus status) => status switch
    {
        RequestStatus.Pending => "status-pending",
        RequestStatus.Closed => "status-closed",
        _ => ""
    };

    private string GetStatusCardClass(RequestStatus status) => status switch
    {
        RequestStatus.Pending => "card-pending",
        RequestStatus.Closed => "card-closed",
        _ => ""
    };
}
