using System.Globalization;
using System.Text.RegularExpressions;
using TrainFinder.Application.Dtos;
using TrainFinder.Application.Parsers.Interfaces;

namespace TrainFinder.Application.Parsers;

public class BdzTimetableParser : IBdzTimetableParser
{
    public List<ParsedTimetableStopDto> Parse(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return new List<ParsedTimetableStopDto>();
        }

        var pointsBlock = ExtractUvPointsBlock(html);

        if (pointsBlock == null)
        {
            return new List<ParsedTimetableStopDto>();
        }

        var rowMatches = Regex.Matches(
            pointsBlock,
            @"<div\s+class=""row""\s*>\s*<div\s+class=""col-2[^""]*""\s*>(.*?)</div>\s*<div\s+class=""col-2[^""]*""\s*>(.*?)</div>\s*<div\s+class=""col-8[^""]*""\s*>(.*?)</div>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);

        var stops = new List<ParsedTimetableStopDto>();
        var order = 0;

        foreach (Match match in rowMatches)
        {
            var arrivalRaw = StripHtml(match.Groups[1].Value).Trim();
            var departureRaw = StripHtml(match.Groups[2].Value).Trim();
            var stationName = StripHtml(match.Groups[3].Value).Trim();

            if (string.IsNullOrWhiteSpace(stationName))
            {
                continue;
            }

            stops.Add(new ParsedTimetableStopDto
            {
                StopOrder = order++,
                StationName = stationName,
                ArrivalTime = ParseTime(arrivalRaw),
                DepartureTime = ParseTime(departureRaw)
            });
        }

        return stops;
    }

    private static string? ExtractUvPointsBlock(string html)
    {
        var startIndex = html.IndexOf("uv-points", StringComparison.OrdinalIgnoreCase);

        if (startIndex < 0)
        {
            return null;
        }

        var endIndex = html.IndexOf("mt-4", startIndex, StringComparison.OrdinalIgnoreCase);

        if (endIndex < 0)
        {
            endIndex = html.Length;
        }

        return html.Substring(startIndex, endIndex - startIndex);
    }

    private static TimeOnly? ParseTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Contains('→') || value.Contains('←') ||
            value.Contains("&rarr;") || value.Contains("&larr;"))
        {
            return null;
        }

        if (TimeOnly.TryParseExact(value, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            return time;
        }

        if (TimeOnly.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
        {
            return time;
        }

        return null;
    }

    private static string StripHtml(string value)
    {
        return Regex.Replace(value, "<.*?>", string.Empty).Trim();
    }
}
