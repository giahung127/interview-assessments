namespace Minesweeper.Core.Models;

public class Cell
{
    public int Row { get; }
    public int Col { get; }
    public bool IsMine { get; set; }
    public int NeighborMines { get; set; }
    public CellVisibility Visibility { get; set; } = CellVisibility.Hidden;
    public bool Exploded { get; set; }

    public Cell(int row, int col)
    {
        Row = row;
        Col = col;
    }
}
