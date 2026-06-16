using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using TrainFinder.Application.Dtos;
using TrainFinder.Application.Parsers.Interfaces;

namespace TrainFinder.Application.Parsers;

public class BdzRadarTrainParser : IBdzRadarTrainParser
{
    public List<ParsedTrainLocationDto> Parse(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return new List<ParsedTrainLocationDto>();
        }

        var trainsJson = ExtractTrainsJson(html);

        if (string.IsNullOrWhiteSpace(trainsJson))
        {
            return new List<ParsedTrainLocationDto>();
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var bdzTrains = JsonSerializer.Deserialize<List<BdzRadarTrainDto>>(trainsJson, options)
                        ?? new List<BdzRadarTrainDto>();

        return bdzTrains
            .Where(x => x.Train.HasValue && x.Lat.HasValue && x.Lng.HasValue)
            .Select(x =>
            {
                var infoWindow = WebUtility.HtmlDecode(x.InfoWindow ?? string.Empty);

                return new ParsedTrainLocationDto
                {
                    TrainNumber = x.Train!.Value,
                    CategoryId = x.CategoryId ?? 0,
                    WagonCount = x.WagCount ?? 0,
                    StationCode = x.Station,
                    StationName = ExtractStationName(infoWindow, "point-blink"),
                    NextStationCode = x.NextStation,
                    NextStationName = ExtractStationName(infoWindow, "point-change"),
                    Latitude = x.Lat!.Value,
                    Longitude = x.Lng!.Value,
                    DelayMinutes = x.Delay ?? 0,
                    TimePlanned = ParseDateTime(x.TimePlanned),
                    ReportedAt = ParseDateTime(x.Date) ?? DateTime.UtcNow
                };
            })
            .ToList();
    }

    private static string? ExtractTrainsJson(string html)
    {
        var startMarker = "var trains =";
        var startIndex = html.IndexOf(startMarker, StringComparison.OrdinalIgnoreCase);

        if (startIndex < 0)
        {
            return null;
        }

        startIndex += startMarker.Length;

        var arrayStartIndex = html.IndexOf('[', startIndex);

        if (arrayStartIndex < 0)
        {
            return null;
        }

        var depth = 0;
        var inString = false;
        var isEscaped = false;

        for (var i = arrayStartIndex; i < html.Length; i++)
        {
            var currentChar = html[i];

            if (isEscaped)
            {
                isEscaped = false;
                continue;
            }

            if (currentChar == '\\')
            {
                isEscaped = true;
                continue;
            }

            if (currentChar == '"')
            {
                inString = !inString;
                continue;
            }

            if (inString)
            {
                continue;
            }

            if (currentChar == '[')
            {
                depth++;
            }
            else if (currentChar == ']')
            {
                depth--;

                if (depth == 0)
                {
                    return html.Substring(arrayStartIndex, i - arrayStartIndex + 1);
                }
            }
        }

        return null;
    }

    private static DateTime? ParseDateTime(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var formats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(
                    value,
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out var result))
            {
                return result;
            }
        }

        return null;
    }

    private static string? ExtractStationName(string infoWindow, string pointClass)
    {
        if (string.IsNullOrWhiteSpace(infoWindow))
        {
            return null;
        }

        var match = Regex.Match(
            infoWindow,
            $@"<div\s+class=""row point {Regex.Escape(pointClass)}"">[\s\S]*?<div\s+class=""col-\d+\s+text-uppercase"">\s*(.*?)\s*</div>",
            RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return null;
        }

        var stationName = StripHtml(match.Groups[1].Value);

        return string.IsNullOrWhiteSpace(stationName)
            ? null
            : stationName.Trim();
    }

    private static string StripHtml(string value)
    {
        return Regex.Replace(value, "<.*?>", string.Empty).Trim();
    }

    private class BdzRadarTrainDto
    {
        [JsonPropertyName("train")]
        public int? Train { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lng")]
        public double? Lng { get; set; }

        [JsonPropertyName("delay")]
        public int? Delay { get; set; }

        [JsonPropertyName("category_id")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("station")]
        public string? Station { get; set; }

        [JsonPropertyName("next_station")]
        public string? NextStation { get; set; }

        [JsonPropertyName("LocNumber")]
        public string? LocNumber { get; set; }

        [JsonPropertyName("WagCount")]
        public int? WagCount { get; set; }

        [JsonPropertyName("TimePlanned")]
        public string? TimePlanned { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("infoWindow")]
        public string? InfoWindow { get; set; }

        [JsonPropertyName("scrollbarItem")]
        public string? ScrollbarItem { get; set; }
    }
}