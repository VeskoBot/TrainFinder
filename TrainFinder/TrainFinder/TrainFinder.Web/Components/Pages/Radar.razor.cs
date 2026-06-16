using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using TrainFinder.Application.DTOs;
using TrainFinder.Application.Helpers;
using TrainFinder.Application.Interfaces;
using TrainFinder.Data.Enums;
namespace TrainFinder.Web.Components.Pages
{
    public partial class Radar : ComponentBase, IDisposable
    {
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Inject]
        private ITrainLocationService TrainLocationService { get; set; } = default!;

        [Inject]
        private ITrainService TrainService { get; set; } = default!;

        [Inject]
        private ITimetableService TimetableService { get; set; } = default!;

        private List<TrainInfoDto> TrainInfoList { get; set; } = new();
        private bool IsLoading { get; set; } = true;
        private int? SelectedTrainNumber { get; set; }
        private bool SidebarOpen { get; set; } = false;

        private void ToggleSidebar() => SidebarOpen = !SidebarOpen;

        private double _touchStartX;
        private const double SwipeThreshold = 60;

        private void OnSidebarTouchStart(TouchEventArgs e)
        {
            if (e.Touches.Length > 0)
                _touchStartX = e.Touches[0].ClientX;
        }

        private void OnSidebarTouchEnd(TouchEventArgs e)
        {
            if (e.ChangedTouches.Length > 0)
            {
                var deltaX = e.ChangedTouches[0].ClientX - _touchStartX;
                if (deltaX < -SwipeThreshold)
                    SidebarOpen = false;
            }
        }

        private void OnMapTouchStart(TouchEventArgs e) { }
        private void OnMapTouchEnd(TouchEventArgs e) { }

        private string? _searchTerm;
        private string? SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                CurrentPage = 1;
                InvokeAsync(UpdateMapPinsAsync);
            }
        }

        private bool FilterDelayed { get; set; } = true;
        private bool FilterOnTime { get; set; } = true;
        private bool FilterFast { get; set; } = true;
        private bool FilterPassenger { get; set; } = true;
        private bool FilterSuburban { get; set; } = true;
        private SortMode DelaySortMode { get; set; } = SortMode.None;

        private int CurrentPage { get; set; } = 1;
        private const int PageSize = 10;

        private IEnumerable<TrainInfoDto> SearchFilteredTrains
        {
            get
            {
                var results = TrainInfoList.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    var term = SearchTerm.Trim().ToLowerInvariant();
                    results = results.Where(t =>
                        t.TrainNumber.ToString().Contains(term) ||
                        (t.StationName?.ToLowerInvariant().Contains(term) ?? false) ||
                        (t.NextStationName?.ToLowerInvariant().Contains(term) ?? false) ||
                        (t.StartStationName?.ToLowerInvariant().Contains(term) ?? false) ||
                        (t.FinalStationName?.ToLowerInvariant().Contains(term) ?? false) ||
                        GetCategoryDisplay(t.Category).ToLowerInvariant().Contains(term) ||
                        t.DelayMinutes.ToString().Contains(term));
                }

                return results;
            }
        }

        private IEnumerable<TrainInfoDto> CategoryFilteredTrains =>
            SearchFilteredTrains.Where(t =>
                (FilterFast && (t.Category == TrainCategory.Fast || t.Category == TrainCategory.InternationalFast)) ||
                (FilterPassenger && t.Category == TrainCategory.Passenger) ||
                (FilterSuburban && t.Category == TrainCategory.SuburbanPassenger) ||
                t.Category == TrainCategory.Unknown);

        private IEnumerable<TrainInfoDto> DelayFilteredTrains =>
            SearchFilteredTrains.Where(t =>
                (FilterDelayed && t.DelayMinutes > 0) ||
                (FilterOnTime && t.DelayMinutes == 0));

        private int DelayedCount => CategoryFilteredTrains.Count(t => t.DelayMinutes > 0);
        private int OnTimeCount => CategoryFilteredTrains.Count(t => t.DelayMinutes == 0);
        private int FastCount => DelayFilteredTrains.Count(t => t.Category == TrainCategory.Fast || t.Category == TrainCategory.InternationalFast);
        private int PassengerCount => DelayFilteredTrains.Count(t => t.Category == TrainCategory.Passenger);
        private int SuburbanCount => DelayFilteredTrains.Count(t => t.Category == TrainCategory.SuburbanPassenger);

        private IEnumerable<TrainInfoDto> FilteredTrains
        {
            get
            {
                var results = SearchFilteredTrains;

                results = results.Where(t =>
                    (FilterDelayed && t.DelayMinutes > 0) ||
                    (FilterOnTime && t.DelayMinutes == 0));

                results = results.Where(t =>
                    (FilterFast && (t.Category == TrainCategory.Fast || t.Category == TrainCategory.InternationalFast)) ||
                    (FilterPassenger && t.Category == TrainCategory.Passenger) ||
                    (FilterSuburban && t.Category == TrainCategory.SuburbanPassenger) ||
                    t.Category == TrainCategory.Unknown);

                return DelaySortMode switch
                {
                    SortMode.Descending => results.OrderByDescending(t => t.DelayMinutes),
                    SortMode.Ascending => results.OrderBy(t => t.DelayMinutes),
                    _ => results.OrderBy(t => t.TrainNumber)
                };
            }
        }

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredTrains.Count() / (double)PageSize));

        private IEnumerable<TrainInfoDto> PaginatedTrains =>
            FilteredTrains.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        private async Task ToggleDelayed()
        {
            FilterDelayed = !FilterDelayed;
            CurrentPage = 1;
            await UpdateMapPinsAsync();
        }

        private async Task ToggleOnTime()
        {
            FilterOnTime = !FilterOnTime;
            CurrentPage = 1;
            await UpdateMapPinsAsync();
        }

        private void CycleSortByDelay()
        {
            DelaySortMode = DelaySortMode switch
            {
                SortMode.None => SortMode.Descending,
                SortMode.Descending => SortMode.Ascending,
                SortMode.Ascending => SortMode.None,
                _ => SortMode.None
            };
            CurrentPage = 1;
        }

        private enum SortMode
        {
            None,
            Descending,
            Ascending
        }

        private async Task ToggleFast()
        {
            FilterFast = !FilterFast;
            CurrentPage = 1;
            await UpdateMapPinsAsync();
        }

        private async Task TogglePassenger()
        {
            FilterPassenger = !FilterPassenger;
            CurrentPage = 1;
            await UpdateMapPinsAsync();
        }

        private async Task ToggleSuburban()
        {
            FilterSuburban = !FilterSuburban;
            CurrentPage = 1;
            await UpdateMapPinsAsync();
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages) CurrentPage++;
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1) CurrentPage--;
        }

        private string GetToggleClass(bool active) =>
            $"filter-toggle {(active ? "active" : "")}";

        private async Task SelectTrain(TrainInfoDto train)
        {
            await JS.InvokeVoidAsync("radarMap.openTrainPopup", train.TrainNumber);
        }

        private DotNetObjectReference<Radar>? _dotNetRef;
        private readonly CancellationTokenSource _cts = new();
        private Timer? _countdownTimer;
        private string NextUpdateIn { get; set; } = "";

        private static DateTime GetNextRoundFiveMinutes()
        {
            var now = DateTime.Now;
            var minutes = now.Minute;
            var nextMinute = (int)(Math.Ceiling(minutes / 5.0) * 5);
            var next = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
                .AddMinutes(nextMinute == 60 ? 60 : nextMinute);
            if (next <= now) next = next.AddMinutes(5);
            return next;
        }

        private void StartCountdownTimer()
        {
            _countdownTimer?.Dispose();
            _countdownTimer = new Timer(_ =>
            {
                var remaining = GetNextRoundFiveMinutes() - DateTime.Now;
                if (remaining.TotalSeconds <= 0)
                {
                    NextUpdateIn = "Обновяване...";
                    InvokeAsync(async () =>
                    {
                        await LoadTrainsAsync();
                        StartCountdownTimer();
                    });
                }
                else
                {
                    NextUpdateIn = $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}";
                    InvokeAsync(StateHasChanged);
                }
            }, null, 0, 1000);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _dotNetRef = DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("radarMap.init", _dotNetRef);
                    await LoadTrainsAsync();
                    StartCountdownTimer();
                }
                catch (JSDisconnectedException) { }
            }
        }

        [JSInvokable]
        public void OnTrainPopupOpened(int trainNumber)
        {
            SelectedTrainNumber = trainNumber;
            StateHasChanged();
        }

        [JSInvokable]
        public void OnTrainPopupClosed()
        {
            SelectedTrainNumber = null;
            StateHasChanged();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _dotNetRef?.Dispose();
            _countdownTimer?.Dispose();
        }

        private async Task LoadTrainsAsync()
        {
            IsLoading = true;
            StateHasChanged();

            var locations = await TrainLocationService.GetRecentCurrentLocationsAsync(CancellationToken.None);

            TrainInfoList.Clear();

            foreach (var location in locations)
            {
                try
                {
                    var train = await TrainService.GetByIdAsync(location.TrainId, CancellationToken.None);

                    if (train != null)
                    {
                        string? startStationName = null;
                        string? finalStationName = null;
                        int totalStops = 0;
                        int passedStops = 0;
                        string? currentStationArrivalTime = null;
                        string? nextStationDepartureTime = null;
                        var timetableStops = new List<TimetableStopInfoDto>();

                        try
                        {
                            var timetable = await TimetableService.GetByTrainIdAsync(train.Id, CancellationToken.None);

                            if (timetable?.Stops.Count > 0)
                            {
                                var orderedStops = timetable.Stops.OrderBy(s => s.StopOrder).ToList();
                                startStationName = orderedStops.First().Station.Name;
                                finalStationName = orderedStops.Last().Station.Name;
                                totalStops = orderedStops.Count;

                                int currentStopIndex = -1;

                                if (location.StationId != null)
                                {
                                    currentStopIndex = orderedStops.FindIndex(s => s.StationId == location.StationId);

                                    if (currentStopIndex < 0 && location.Station?.Name != null)
                                    {
                                        var currentName = location.Station.Name;
                                        currentStopIndex = orderedStops.FindIndex(s =>
                                            string.Equals(s.Station.Name, currentName, StringComparison.OrdinalIgnoreCase));
                                    }

                                    if (currentStopIndex >= 0)
                                    {
                                        passedStops = currentStopIndex + 1;
                                    }
                                    else
                                    {
                                        var now = TimeOnly.FromDateTime(TimeHelper.GetEasternEuropeanTime());
                                        var adjustedNow = now.AddMinutes(-location.DelayMinutes);

                                        for (int i = orderedStops.Count - 1; i >= 0; i--)
                                        {
                                            var stopTime = orderedStops[i].DepartureTime ?? orderedStops[i].ArrivalTime;
                                            if (stopTime != null && stopTime <= adjustedNow)
                                            {
                                                passedStops = i + 1;
                                                currentStopIndex = i;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (currentStopIndex >= 0)
                                {
                                    var curStop = orderedStops[currentStopIndex];
                                    var baseTime = curStop.ArrivalTime ?? curStop.DepartureTime;
                                    if (baseTime != null)
                                        currentStationArrivalTime = baseTime.Value.AddMinutes(location.DelayMinutes).ToString("HH:mm");

                                    if (currentStopIndex + 1 < orderedStops.Count)
                                    {
                                        var nextStop = orderedStops[currentStopIndex + 1];
                                        var nextBase = nextStop.ArrivalTime ?? nextStop.DepartureTime;
                                        if (nextBase != null)
                                            nextStationDepartureTime = nextBase.Value.AddMinutes(location.DelayMinutes).ToString("HH:mm");
                                    }
                                }

                                for (int i = 0; i < orderedStops.Count; i++)
                                {
                                    var s = orderedStops[i];
                                    timetableStops.Add(new TimetableStopInfoDto
                                    {
                                        StopOrder = s.StopOrder,
                                        StationName = s.Station.Name,
                                        ArrivalTime = s.ArrivalTime?.AddMinutes(location.DelayMinutes).ToString("HH:mm"),
                                        DepartureTime = s.DepartureTime?.AddMinutes(location.DelayMinutes).ToString("HH:mm"),
                                        IsCurrent = i == currentStopIndex,
                                        IsPassed = i < passedStops
                                    });
                                }
                            }
                        }
                        catch (InvalidOperationException)
                        {

                        }

                        TrainInfoList.Add(new TrainInfoDto
                        {
                            TrainNumber = train.TrainNumber,
                            Category = train.Category,
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            DelayMinutes = location.DelayMinutes,
                            WagonCount = train.WagonCount,
                            StationName = location.Station?.Name,
                            NextStationName = location.NextStation?.Name,
                            StartStationName = startStationName,
                            FinalStationName = finalStationName,
                            LastReportedAt = location.LastReportedAt,
                            TotalStops = totalStops,
                            PassedStops = passedStops,
                            CurrentStationArrivalTime = currentStationArrivalTime,
                            NextStationDepartureTime = nextStationDepartureTime,
                            TimetableStops = timetableStops
                        });
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }

            if (_cts.IsCancellationRequested) return;
            await UpdateMapPinsAsync();
            IsLoading = false;
            StateHasChanged();
        }

        private async Task UpdateMapPinsAsync()
        {
            if (_cts.IsCancellationRequested) return;
            var pins = FilteredTrains.Select(t => new
            {
                latitude = t.Latitude,
                longitude = t.Longitude,
                trainNumber = t.TrainNumber,
                delayMinutes = t.DelayMinutes,
                category = (int)t.Category,
                stationName = t.StationName ?? "",
                nextStationName = t.NextStationName ?? "",
                startStationName = t.StartStationName ?? "",
                finalStationName = t.FinalStationName ?? "",
                lastReportedAt = t.LastReportedAt?.ToString("HH:mm dd.MM.yyyy") ?? "",
                progressPercent = t.ProgressPercent,
                passedStops = t.PassedStops,
                totalStops = t.TotalStops,
                categoryName = GetCategoryFullName(t.Category),
                wagonCount = t.WagonCount,
                currentStationArrivalTime = t.CurrentStationArrivalTime ?? "",
                nextStationDepartureTime = t.NextStationDepartureTime ?? "",
                timetableStops = t.TimetableStops.Select(s => new
                {
                    stopOrder = s.StopOrder,
                    stationName = s.StationName,
                    arrivalTime = s.ArrivalTime ?? "",
                    departureTime = s.DepartureTime ?? "",
                    isCurrent = s.IsCurrent,
                    isPassed = s.IsPassed
                }).ToList()
            }).ToList();

            await JS.InvokeVoidAsync("radarMap.updateTrains", pins);
        }

        private string GetCategoryDisplay(TrainCategory category)
        {
            return category switch
            {
                TrainCategory.InternationalFast => "МБВ",
                TrainCategory.Fast => "БВ",
                TrainCategory.SuburbanPassenger => "КПВ",
                TrainCategory.Passenger => "ПВ",
                _ => "Неоточнен"
            };
        }

        private string GetCategoryFullName(TrainCategory category)
        {
            return category switch
            {
                TrainCategory.InternationalFast => "Международен бърз влак",
                TrainCategory.Fast => "Бърз влак",
                TrainCategory.SuburbanPassenger => "Крайградски пътнически влак",
                TrainCategory.Passenger => "Пътнически влак",
                _ => "Неоточнен"
            };
        }

        private string GetCategoryCardClass(TrainCategory category)
        {
            return category switch
            {
                TrainCategory.Fast => "card-fast",
                TrainCategory.InternationalFast => "card-international",
                TrainCategory.Passenger => "card-passenger",
                TrainCategory.SuburbanPassenger => "card-suburban",
                _ => ""
            };
        }

        private string GetCategoryBadgeClass(TrainCategory category)
        {
            return category switch
            {
                TrainCategory.Fast => "category-fast",
                TrainCategory.InternationalFast => "category-international",
                TrainCategory.Passenger => "category-passenger",
                TrainCategory.SuburbanPassenger => "category-suburban",
                _ => "category-unknown"
            };
        }
    }
}