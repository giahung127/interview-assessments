using Minesweeper.App.ViewModels;
using Minesweeper.Core.Engine;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using Xunit;

namespace Minesweeper.Tests.ViewModels;

public class GameViewModelTests
{
    [Fact]
    public void StartGame_PopulatesCells_Correctly()
    {
        var engine = new GameEngine(new StandardBoardGenerator(), new SystemClockService());
        var vm = new GameViewModel(
            engine,
            new InMemorySettingsStore(),
            new InMemoryStatsStore(),
            new FixedDailyChallengeService());

        Assert.Equal(GameStatus.NotStarted, vm.Status);
        Assert.Equal(81, vm.Cells.Count);
        Assert.Equal(9, vm.BoardWidth);
        Assert.Equal(10, vm.MinesLeft);
    }

    private sealed class InMemorySettingsStore : ISettingsStore
    {
        public UserSettings Settings { get; private set; } = UserSettings.Default;

        public UserSettings Load() => Settings;

        public void Save(UserSettings settings)
        {
            Settings = settings;
        }
    }

    private sealed class InMemoryStatsStore : IStatsStore
    {
        public void RecordGame(GameResult result)
        {
        }

        public PlayerStatsSummary GetSummary() => PlayerStatsSummary.Empty;

        public BestTimes GetBestTimes() => BestTimes.Empty;
    }

    private sealed class FixedDailyChallengeService : IDailyChallengeService
    {
        public int GetDailySeed(DateOnly localDate) => 12345;
    }
}
