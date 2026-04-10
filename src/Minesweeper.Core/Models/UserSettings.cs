namespace Minesweeper.Core.Models;

public record UserSettings(
    string LastSelectedDifficulty,
    bool HighContrastEnabled,
    bool ReducedMotionEnabled
)
{
    public static UserSettings Default => new(
        DifficultyPreset.Beginner.Name,
        HighContrastEnabled: false,
        ReducedMotionEnabled: false);
}
