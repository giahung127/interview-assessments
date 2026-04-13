using Minesweeper.Core.Engine;
using Minesweeper.App.Services;

namespace Minesweeper.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public GameViewModel GameViewModel { get; }

    public MainWindowViewModel()
    {
        var boardGenerator = new StandardBoardGenerator();
        var clockService = new SystemClockService();
        var engine = new GameEngine(boardGenerator, clockService);

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var databasePath = Path.Combine(localAppData, "Minesweeper", "minesweeper.db");
        var storage = new SqliteStorage(databasePath);
        var settingsStore = new SqliteSettingsStore(storage);
        var statsStore = new SqliteStatsStore(storage);
        var dailyChallengeService = new LocalDateDailyChallengeService();

        GameViewModel = new GameViewModel(engine, settingsStore, statsStore, dailyChallengeService);
    }
}
