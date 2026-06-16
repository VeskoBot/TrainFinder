using TrainFinder.Application.Dtos;

namespace TrainFinder.Application.Parsers.Interfaces;

public interface IBdzRadarTrainParser
{
    List<ParsedTrainLocationDto> Parse(string html);
}