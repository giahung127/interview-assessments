namespace Minesweeper.Core.Models;

public record PlayerStatsSummary(
    int GamesPlayed,
    int GamesWon,
    int CurrentWinStreak,
    int BestWinStreak,
    PerformanceStatsSummary Performance
)
{
    public double WinRate => GamesPlayed == 0 ? 0 : (double)GamesWon / GamesPlayed;

    public static PlayerStatsSummary Empty => new(
        GamesPlayed: 0,
        GamesWon: 0,
        CurrentWinStreak: 0,
        BestWinStreak: 0,
        Performance: new PerformanceStatsSummary(0, 0, 0));
}
