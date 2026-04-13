namespace Minesweeper.Core.Interfaces;

using Minesweeper.Core.Models;

public interface IStatsStore
{
    void RecordGame(GameResult result);
    PlayerStatsSummary GetSummary();
    BestTimes GetBestTimes();
}
