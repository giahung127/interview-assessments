namespace Minesweeper.Core.Models;

public class Board
{
    public int Rows { get; }
    public int Cols { get; }
    public int MineCount { get; }
    private readonly Cell[,] _cells;

    public Board(int rows, int cols, int mineCount)
    {
        Rows = rows;
        Cols = cols;
        MineCount = mineCount;
        _cells = new Cell[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                _cells[r, c] = new Cell(r, c);
            }
        }
    }

    public Cell GetCell(int row, int col)
    {
        return _cells[row, col];
    }

    public bool IsInBounds(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    public IEnumerable<Cell> GetAllCells()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                yield return _cells[r, c];
            }
        }
    }
}
