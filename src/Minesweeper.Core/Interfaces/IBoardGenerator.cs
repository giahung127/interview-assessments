namespace Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

public interface IBoardGenerator
{
    Board Generate(int rows, int cols, int mineCount, IRandomProvider random);
    void RelocateMine(Board board, int row, int col, IRandomProvider random);
    void ComputeNeighborMines(Board board);
}
