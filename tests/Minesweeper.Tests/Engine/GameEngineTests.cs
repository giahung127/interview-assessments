using Xunit;
using Minesweeper.Core.Engine;
using Minesweeper.Core.Models;

namespace Minesweeper.Tests.Engine;

public class GameEngineTests
{
    [Fact]
    public void StartNewGame_ResetsState()
    {
        var engine = new GameEngine(new StandardBoardGenerator(), new SystemClockService());
        engine.StartNewGame(DifficultyPreset.Beginner);

        var snapshot = engine.GetSnapshot();
        Assert.Equal(GameStatus.NotStarted, snapshot.Status);
        Assert.Equal(10, snapshot.MinesLeft);
    }

    [Fact]
    public void FirstReveal_NeverHitsMine()
    {
        var engine = new GameEngine(new StandardBoardGenerator(), new SystemClockService());
        engine.StartNewGame(new DifficultyPreset("Custom", 5, 5, 24));
        
        engine.RevealCell(2, 2);
        var snapshot = engine.GetSnapshot();
        
        Assert.NotEqual(GameStatus.Lost, snapshot.Status);
        
        int mines = 0;
        foreach (var cell in snapshot.Cells)
        {
            if (cell.IsMine) mines++;
        }
        Assert.Equal(24, mines);
    }
}
