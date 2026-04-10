using Minesweeper.Core.Models;

namespace Minesweeper.Core.Interfaces;

public interface IStatsStore
{
    void RecordGame(GameResult result);
    PlayerStatsSummary GetSummary();
    BestTimes GetBestTimes();
}
