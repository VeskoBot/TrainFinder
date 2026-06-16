using TrainFinder.Application.Interfaces;

namespace TrainFinder.Infrastructure.Clients;

public class BdzRadarClient : IBdzRadarClient
{
    private readonly HttpClient _httpClient;

    public BdzRadarClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetRadarHtmlAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("bg/refresh", cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}