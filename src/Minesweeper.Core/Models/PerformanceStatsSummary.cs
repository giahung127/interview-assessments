namespace Minesweeper.Core.Models;

public record PerformanceStatsSummary(
    double AverageSolveSeconds,
    double AverageActionsPerWin,
    double AverageActionsPerSecond
);
