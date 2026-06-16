using TrainFinder.Application.Interfaces;

namespace TrainFinder.Infrastructure.Clients;

public class BdzTimetableClient : IBdzTimetableClient
{
    private readonly HttpClient _httpClient;

    public BdzTimetableClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTimetableHtmlAsync(int trainNumber, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"bg/train-info/{trainNumber}", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
