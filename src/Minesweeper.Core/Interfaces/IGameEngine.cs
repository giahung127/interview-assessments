namespace Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

public interface IGameEngine
{
    void StartNewGame(DifficultyPreset preset, int? seed = null);
    void RevealCell(int row, int col);
    void ToggleFlag(int row, int col);
    void ChordReveal(int row, int col);
    GameSessionSnapshot GetSnapshot();
}
