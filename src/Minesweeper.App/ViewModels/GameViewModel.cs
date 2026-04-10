using Avalonia.Threading;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Minesweeper.App.ViewModels;

public class GameViewModel : ViewModelBase
{
    private readonly IGameEngine _engine;
    private readonly ISettingsStore _settingsStore;
    private readonly IStatsStore _statsStore;
    private readonly IDailyChallengeService _dailyChallengeService;
    private readonly DispatcherTimer _timer;

    private DifficultyPreset _currentPreset = DifficultyPreset.Beginner;
    private int _actionCount;
    private int? _currentSeed;
    private bool _currentIsDailyChallenge;
    private bool _recordedCurrentGame;
    private bool _isLoadingSettings;

    public DifficultyPreset[] AvailablePresets { get; } =
    [
        DifficultyPreset.Beginner,
        DifficultyPreset.Intermediate,
        DifficultyPreset.Expert
    ];

    public DifficultyPreset SelectedPreset
    {
        get => _currentPreset;
        set
        {
            if (value == _currentPreset)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _currentPreset, value);
            PersistSettings();

            if (SelectDifficultyCommand.CanExecute(value))
            {
                SelectDifficultyCommand.Execute(value);
            }
        }
    }

    private int _minesLeft;
    public int MinesLeft
    {
        get => _minesLeft;
        set => this.RaiseAndSetIfChanged(ref _minesLeft, value);
    }

    private int _elapsedSeconds;
    public int ElapsedSeconds
    {
        get => _elapsedSeconds;
        set => this.RaiseAndSetIfChanged(ref _elapsedSeconds, value);
    }

    private GameStatus _status;
    public GameStatus Status
    {
        get => _status;
        set
        {
            this.RaiseAndSetIfChanged(ref _status, value);
            this.RaisePropertyChanged(nameof(IsGameOver));
            this.RaisePropertyChanged(nameof(GameOverMessage));
        }
    }

    public bool IsGameOver => Status is GameStatus.Won or GameStatus.Lost;

    public string GameOverMessage => Status switch
    {
        GameStatus.Won => "You Win!",
        GameStatus.Lost => "Game Over",
        _ => string.Empty
    };

    private int _boardWidth;
    public int BoardWidth
    {
        get => _boardWidth;
        set => this.RaiseAndSetIfChanged(ref _boardWidth, value);
    }

    private bool _highContrastEnabled;
    public bool HighContrastEnabled
    {
        get => _highContrastEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _highContrastEnabled, value);
            PersistSettings();
            RefreshCellStyles();
        }
    }

    private bool _reducedMotionEnabled;
    public bool ReducedMotionEnabled
    {
        get => _reducedMotionEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref _reducedMotionEnabled, value);
            PersistSettings();
        }
    }

    private string _statsSummaryText = string.Empty;
    public string StatsSummaryText
    {
        get => _statsSummaryText;
        set => this.RaiseAndSetIfChanged(ref _statsSummaryText, value);
    }

    private string _bestTimesText = string.Empty;
    public string BestTimesText
    {
        get => _bestTimesText;
        set => this.RaiseAndSetIfChanged(ref _bestTimesText, value);
    }

    private string _performanceSummaryText = string.Empty;
    public string PerformanceSummaryText
    {
        get => _performanceSummaryText;
        set => this.RaiseAndSetIfChanged(ref _performanceSummaryText, value);
    }

    public ObservableCollection<CellViewModel> Cells { get; } = new();

    public ICommand RestartCommand { get; }
    public ICommand SelectDifficultyCommand { get; }
    public ICommand PlayDailyChallengeCommand { get; }
    public ICommand QuickRematchCommand { get; }

    public GameViewModel(
        IGameEngine engine,
        ISettingsStore settingsStore,
        IStatsStore statsStore,
        IDailyChallengeService dailyChallengeService)
    {
        _engine = engine;
        _settingsStore = settingsStore;
        _statsStore = statsStore;
        _dailyChallengeService = dailyChallengeService;

        RestartCommand = ReactiveCommand.Create(() => StartGame(_currentPreset));
        SelectDifficultyCommand = ReactiveCommand.Create<DifficultyPreset>(preset =>
        {
            _currentPreset = preset;
            PersistSettings();
            StartGame(preset);
        });
        PlayDailyChallengeCommand = ReactiveCommand.Create(PlayDailyChallenge);
        QuickRematchCommand = ReactiveCommand.Create(QuickRematch);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += (_, _) => UpdateFromSnapshot();

        ApplyPersistedSettings();
        RefreshStatsSummary();
        StartGame(_currentPreset);
    }

    public void RegisterAction()
    {
        _actionCount++;
    }

    public void StartGame(DifficultyPreset preset, int? seed = null, bool isDailyChallenge = false)
    {
        _currentPreset = preset;
        this.RaisePropertyChanged(nameof(SelectedPreset));

        _currentSeed = seed ?? Random.Shared.Next(1, int.MaxValue);
        _currentIsDailyChallenge = isDailyChallenge;
        _actionCount = 0;
        _recordedCurrentGame = false;

        _engine.StartNewGame(preset, _currentSeed);
        _timer.Start();
        UpdateFromSnapshot(forceRebuild: true);
    }

    public void UpdateFromSnapshot(bool forceRebuild = false)
    {
        var snapshot = _engine.GetSnapshot();
        var previousStatus = Status;

        MinesLeft = snapshot.MinesLeft;
        ElapsedSeconds = (int)snapshot.ElapsedTime.TotalSeconds;
        Status = snapshot.Status;

        if (IsGameOver)
        {
            _timer.Stop();
        }

        if (!_recordedCurrentGame && Status is GameStatus.Won or GameStatus.Lost)
        {
            _statsStore.RecordGame(new GameResult(
                PlayedAtUtc: DateTime.UtcNow,
                Difficulty: _currentPreset,
                DidWin: Status == GameStatus.Won,
                ElapsedSeconds: ElapsedSeconds,
                ActionCount: _actionCount,
                Seed: _currentSeed,
                IsDailyChallenge: _currentIsDailyChallenge));

            _recordedCurrentGame = true;
            RefreshStatsSummary();
        }

        if (forceRebuild || Cells.Count != snapshot.Cells.Count)
        {
            BoardWidth = snapshot.Cols;
            Cells.Clear();
            foreach (var cell in snapshot.Cells)
            {
                Cells.Add(new CellViewModel(cell, _engine, this));
            }
        }
        else
        {
            for (var i = 0; i < snapshot.Cells.Count; i++)
            {
                Cells[i].Update(snapshot.Cells[i]);
            }
        }

        if (previousStatus != Status && IsGameOver)
        {
            RefreshCellStyles();
        }
    }

    private void ApplyPersistedSettings()
    {
        _isLoadingSettings = true;
        try
        {
            var settings = _settingsStore.Load();
            _currentPreset = GetPresetByName(settings.LastSelectedDifficulty);
            HighContrastEnabled = settings.HighContrastEnabled;
            ReducedMotionEnabled = settings.ReducedMotionEnabled;
            this.RaisePropertyChanged(nameof(SelectedPreset));
        }
        finally
        {
            _isLoadingSettings = false;
        }
    }

    private void PersistSettings()
    {
        if (_isLoadingSettings)
        {
            return;
        }

        _settingsStore.Save(new UserSettings(
            LastSelectedDifficulty: _currentPreset.Name,
            HighContrastEnabled: HighContrastEnabled,
            ReducedMotionEnabled: ReducedMotionEnabled));
    }

    private void RefreshStatsSummary()
    {
        var summary = _statsStore.GetSummary();
        var bestTimes = _statsStore.GetBestTimes();

        StatsSummaryText = $"Played: {summary.GamesPlayed}  Win rate: {summary.WinRate:P0}  Streak: {summary.CurrentWinStreak}/{summary.BestWinStreak}";
        BestTimesText = $"Best times - B: {FormatSeconds(bestTimes.BeginnerSeconds)}, I: {FormatSeconds(bestTimes.IntermediateSeconds)}, E: {FormatSeconds(bestTimes.ExpertSeconds)}";
        PerformanceSummaryText = $"Avg solve: {summary.Performance.AverageSolveSeconds:F1}s  Avg actions/win: {summary.Performance.AverageActionsPerWin:F1}  Actions/s: {summary.Performance.AverageActionsPerSecond:F2}";
    }

    private void PlayDailyChallenge()
    {
        var localDate = DateOnly.FromDateTime(DateTime.Now);
        var seed = _dailyChallengeService.GetDailySeed(localDate);
        StartGame(DifficultyPreset.Intermediate, seed, isDailyChallenge: true);
        PersistSettings();
    }

    private void QuickRematch()
    {
        if (_currentSeed.HasValue)
        {
            StartGame(_currentPreset, _currentSeed.Value, _currentIsDailyChallenge);
            return;
        }

        StartGame(_currentPreset);
    }

    private DifficultyPreset GetPresetByName(string? presetName)
    {
        foreach (var preset in AvailablePresets)
        {
            if (string.Equals(preset.Name, presetName, StringComparison.OrdinalIgnoreCase))
            {
                return preset;
            }
        }

        return DifficultyPreset.Beginner;
    }

    private static string FormatSeconds(int? value)
    {
        return value.HasValue ? $"{value.Value}s" : "-";
    }

    private void RefreshCellStyles()
    {
        foreach (var cell in Cells)
        {
            cell.RefreshVisualState();
        }
    }
}
