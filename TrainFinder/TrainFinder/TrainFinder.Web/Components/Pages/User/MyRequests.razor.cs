using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel.DataAnnotations;
using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Entities;
using TrainFinder.Data.Enums;

namespace TrainFinder.Web.Components.Pages.User;

public partial class MyRequests
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

    private bool FilterDraft { get; set; } = true;
    private bool FilterPending { get; set; } = true;
    private bool FilterClosed { get; set; } = true;

    private int CurrentPage { get; set; } = 1;
    private const int PageSize = 10;

    private int DraftCount => FilteredBySearch.Count(r => r.Status == RequestStatus.Draft);
    private int PendingCount => FilteredBySearch.Count(r => r.Status == RequestStatus.Pending);
    private int ClosedCount => FilteredBySearch.Count(r => r.Status == RequestStatus.Closed);

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
                    r.Content.ToLowerInvariant().Contains(term));
            }
            return results;
        }
    }

    private IEnumerable<Request> FilteredRequests
    {
        get
        {
            return FilteredBySearch.Where(r =>
                (FilterDraft && r.Status == RequestStatus.Draft) ||
                (FilterPending && r.Status == RequestStatus.Pending) ||
                (FilterClosed && r.Status == RequestStatus.Closed));
        }
    }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredRequests.Count() / (double)PageSize));

    private IEnumerable<Request> PaginatedRequests =>
        FilteredRequests.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private void ToggleDraft() { FilterDraft = !FilterDraft; CurrentPage = 1; }
    private void TogglePending() { FilterPending = !FilterPending; CurrentPage = 1; }
    private void ToggleClosed() { FilterClosed = !FilterClosed; CurrentPage = 1; }
    private void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }
    private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }

    private void SelectRequest(Request request)
    {
        SelectedRequestId = request.Id;
        ShowForm = false;

        if (request.Status == RequestStatus.Draft)
        {
            FormModel = new RequestFormModel
            {
                Title = request.Title,
                Content = request.Content
            };
        }
    }

    private bool ShowForm { get; set; } = true;
    private RequestFormModel FormModel { get; set; } = new();
    private Guid? _currentUserId;
    private bool _isSaving;
    private bool _isSubmitting;

    private async Task LoadRequestsAsync()
    {
        if (_currentUserId == null) return;

        var requests = await RequestService.GetByUserIdAsync(_currentUserId.Value, CancellationToken.None);
        RequestList = requests.ToList();
    }

    private void ShowNewForm()
    {
        ShowForm = true;
        SelectedRequestId = null;
        FormModel = new();
    }

    private async Task DeleteRequest()
    {
        if (SelectedRequest == null || _isSaving) return;

        _isSaving = true;
        try
        {
            await RequestService.DeleteAsync(SelectedRequest.Id, CancellationToken.None);

            RequestList.Remove(SelectedRequest);
            SelectedRequestId = null;
            ShowForm = true;
            FormModel = new();
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task SaveDraft()
    {
        if (string.IsNullOrWhiteSpace(FormModel.Title) && string.IsNullOrWhiteSpace(FormModel.Content))
            return;

        if (SelectedRequest != null && SelectedRequest.Status == RequestStatus.Draft)
        {
            await UpdateRequestAsync(RequestStatus.Draft);
        }
        else if (ShowForm)
        {
            await SaveRequestAsync(RequestStatus.Draft);
        }
    }

    private async Task SaveDraftOnLeave()
    {
        if (_isSaving || _isSubmitting) return;
        if (string.IsNullOrWhiteSpace(FormModel.Title) && string.IsNullOrWhiteSpace(FormModel.Content))
            return;

        if (SelectedRequest != null && SelectedRequest.Status == RequestStatus.Draft)
        {
            await UpdateRequestAsync(RequestStatus.Draft);
        }
        else if (ShowForm)
        {
            await SaveRequestAsync(RequestStatus.Draft);
        }
    }

    private async Task SubmitRequest()
    {
        _isSubmitting = true;
        try
        {
            if (SelectedRequest != null && SelectedRequest.Status == RequestStatus.Draft)
            {
                await UpdateRequestAsync(RequestStatus.Pending);
            }
            else if (ShowForm)
            {
                await SaveRequestAsync(RequestStatus.Pending);
            }
            ShowForm = false;
            SelectedRequestId = null;
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task SaveRequestAsync(RequestStatus status)
    {
        if (_currentUserId == null || _isSaving) return;

        _isSaving = true;
        try
        {
            var request = await RequestService.CreateAsync(_currentUserId.Value, FormModel.Title ?? "", FormModel.Content ?? "", CancellationToken.None);
            request.Status = status;
            await RequestService.UpdateAsync(request, CancellationToken.None);

            RequestList.Insert(0, request);
            FormModel = new();

            if (status == RequestStatus.Pending)
            {
                await LoadRequestsAsync();
            }
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task UpdateRequestAsync(RequestStatus status)
    {
        if (SelectedRequest == null || _isSaving) return;

        _isSaving = true;
        try
        {
            SelectedRequest.Title = FormModel.Title ?? "";
            SelectedRequest.Content = FormModel.Content ?? "";
            SelectedRequest.Status = status;
            await RequestService.UpdateAsync(SelectedRequest, CancellationToken.None);

            var index = RequestList.FindIndex(r => r.Id == SelectedRequest.Id);
            if (index >= 0)
            {
                RequestList[index] = SelectedRequest;
            }

            if (status == RequestStatus.Pending)
            {
                FormModel = new();
                await LoadRequestsAsync();
            }
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

            var user = await UserService.GetByExternalObjectIdAsync(oid, CancellationToken.None);
            if (user == null || (user.Role.Name != "User" && user.Role.Name != "Admin"))
            {
                NavigationManager.NavigateTo("/", replace: true);
                return;
            }

            _currentUserId = user.Id;
            await LoadRequestsAsync();
        }
        catch { }

        IsLoading = false;
    }

    private string GetToggleClass(bool active) => $"filter-toggle {(active ? "active" : "")}";

    private string GetStatusDisplay(RequestStatus status) => status switch
    {
        RequestStatus.Draft => "Чернова",
        RequestStatus.Pending => "Изпратена",
        RequestStatus.Closed => "Затворена",
        _ => "Неизвестен"
    };

    private string GetStatusBadgeClass(RequestStatus status) => status switch
    {
        RequestStatus.Draft => "status-draft",
        RequestStatus.Pending => "status-pending",
        RequestStatus.Closed => "status-closed",
        _ => ""
    };

    private string GetStatusCardClass(RequestStatus status) => status switch
    {
        RequestStatus.Draft => "card-draft",
        RequestStatus.Pending => "card-pending",
        RequestStatus.Closed => "card-closed",
        _ => ""
    };

    public class RequestFormModel
    {
        [Required(ErrorMessage = "Заглавието е задължително")]
        [MaxLength(200, ErrorMessage = "Максимум 200 символа")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Съдържанието е задължително")]
        public string? Content { get; set; }
    }
}
