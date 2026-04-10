namespace Minesweeper.Core.Models;

public class GameSession
{
    public Board Board { get; }
    public GameStatus Status { get; set; }
    public int MinesLeft { get; set; }

    public GameSession(Board board)
    {
        Board = board;
        Status = GameStatus.NotStarted;
        MinesLeft = board.MineCount;
    }
}
