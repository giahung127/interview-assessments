namespace Minesweeper.Core.Models;

public record DifficultyPreset(string Name, int Rows, int Cols, int MineCount)
{
    public static readonly DifficultyPreset Beginner = new("Beginner", 9, 9, 10);
    public static readonly DifficultyPreset Intermediate = new("Intermediate", 16, 16, 40);
    public static readonly DifficultyPreset Expert = new("Expert", 16, 30, 99);
}
