using Xunit;
using Minesweeper.Core.Engine;

namespace Minesweeper.Tests.Engine;

public class BoardGeneratorTests
{
    [Fact]
    public void Generate_CreatesBoardWithCorrectExactMines()
    {
        var gen = new StandardBoardGenerator();
        var random = new DotNetRandomProvider();
        var board = gen.Generate(10, 10, 15, random);

        int mineCount = 0;
        foreach (var cell in board.GetAllCells())
        {
            if (cell.IsMine) mineCount++;
        }

        Assert.Equal(15, mineCount);
    }

    [Fact]
    public void ComputeNeighborMines_IsCorrect()
    {
        var gen = new StandardBoardGenerator();
        var random = new DotNetRandomProvider(42);
        var board = gen.Generate(5, 5, 5, random);

        foreach (var cell in board.GetAllCells())
        {
            if (!cell.IsMine)
            {
                int expectedMines = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        if (board.IsInBounds(cell.Row + i, cell.Col + j) &&
                            board.GetCell(cell.Row + i, cell.Col + j).IsMine)
                        {
                            expectedMines++;
                        }
                    }
                }
                Assert.Equal(expectedMines, cell.NeighborMines);
            }
        }
    }
}
