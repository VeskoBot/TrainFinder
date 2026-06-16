using TrainFinder.Application.Dtos;

namespace TrainFinder.Application.Parsers.Interfaces;

public interface IBdzTimetableParser
{
    List<ParsedTimetableStopDto> Parse(string html);
}
