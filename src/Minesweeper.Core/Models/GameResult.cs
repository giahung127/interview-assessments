namespace Minesweeper.Core.Models;

public record GameResult(
    DateTime PlayedAtUtc,
    DifficultyPreset Difficulty,
    bool DidWin,
    int ElapsedSeconds,
    int ActionCount,
    int? Seed,
    bool IsDailyChallenge
);
