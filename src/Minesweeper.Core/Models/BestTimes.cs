namespace Minesweeper.Core.Models;

public record BestTimes(
    int? BeginnerSeconds,
    int? IntermediateSeconds,
    int? ExpertSeconds
)
{
    public static BestTimes Empty => new(null, null, null);
}
