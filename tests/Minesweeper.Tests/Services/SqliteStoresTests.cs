using Minesweeper.App.Services;
using Minesweeper.Core.Models;
using Xunit;

namespace Minesweeper.Tests.Services;

public class SqliteStoresTests
{
    [Fact]
    public void SettingsStore_RoundTrips_UserSettings()
    {
        using var fixture = new SqliteTestFixture();
        var store = new SqliteSettingsStore(fixture.Storage);

        var expected = new UserSettings(
            LastSelectedDifficulty: DifficultyPreset.Expert.Name,
            HighContrastEnabled: true);

        store.Save(expected);
        var actual = store.Load();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void StatsStore_Computes_Summary_And_BestTimes()
    {
        using var fixture = new SqliteTestFixture();
        var store = new SqliteStatsStore(fixture.Storage);

        store.RecordGame(new GameResult(
            PlayedAtUtc: new DateTime(2026, 4, 8, 1, 0, 0, DateTimeKind.Utc),
            Difficulty: DifficultyPreset.Beginner,
            DidWin: true,
            ElapsedSeconds: 30,
            ActionCount: 60,
            Seed: 100,
            IsDailyChallenge: false));

        store.RecordGame(new GameResult(
            PlayedAtUtc: new DateTime(2026, 4, 8, 2, 0, 0, DateTimeKind.Utc),
            Difficulty: DifficultyPreset.Beginner,
            DidWin: false,
            ElapsedSeconds: 15,
            ActionCount: 20,
            Seed: 200,
            IsDailyChallenge: false));

        store.RecordGame(new GameResult(
            PlayedAtUtc: new DateTime(2026, 4, 8, 3, 0, 0, DateTimeKind.Utc),
            Difficulty: DifficultyPreset.Intermediate,
            DidWin: true,
            ElapsedSeconds: 80,
            ActionCount: 120,
            Seed: 300,
            IsDailyChallenge: true));

        var summary = store.GetSummary();
        var bestTimes = store.GetBestTimes();

        Assert.Equal(3, summary.GamesPlayed);
        Assert.Equal(2, summary.GamesWon);
        Assert.Equal(1, summary.CurrentWinStreak);
        Assert.Equal(1, summary.BestWinStreak);
        Assert.Equal(2d / 3d, summary.WinRate, 3);
        Assert.Equal(55d, summary.Performance.AverageSolveSeconds, 3);

        Assert.Equal(30, bestTimes.BeginnerSeconds);
        Assert.Equal(80, bestTimes.IntermediateSeconds);
        Assert.Null(bestTimes.ExpertSeconds);
    }

    private sealed class SqliteTestFixture : IDisposable
    {
        public SqliteStorage Storage { get; }
        private readonly string _dbPath;

        public SqliteTestFixture()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "MinesweeperTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            _dbPath = Path.Combine(tempDirectory, "stats.db");
            Storage = new SqliteStorage(_dbPath);
        }

        public void Dispose()
        {
            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
