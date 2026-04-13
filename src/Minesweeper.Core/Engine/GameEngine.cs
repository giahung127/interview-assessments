namespace Minesweeper.Core.Engine;

using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

public class GameEngine : IGameEngine
{
    private readonly IBoardGenerator _boardGenerator;
    private readonly IClockService _clockService;
    private GameSession? _session;
    private IRandomProvider _random = new DotNetRandomProvider();

    public GameEngine(IBoardGenerator boardGenerator, IClockService clockService)
    {
        _boardGenerator = boardGenerator;
        _clockService = clockService;
    }

    public void StartNewGame(DifficultyPreset preset, int? seed = null)
    {
        _random = seed.HasValue ? new DotNetRandomProvider(seed.Value) : new DotNetRandomProvider();

        var board = _boardGenerator.Generate(preset.Rows, preset.Cols, preset.MineCount, _random);
        _session = new GameSession(board);

        _clockService.Reset();
    }

    public void RevealCell(int row, int col)
    {
        if (_session == null || _session.Status == GameStatus.Won || _session.Status == GameStatus.Lost)
            return;

        var cell = _session.Board.GetCell(row, col);
        if (cell.Visibility != CellVisibility.Hidden)
            return;

        if (_session.Status == GameStatus.NotStarted)
        {
            _session.Status = GameStatus.InProgress;
            _clockService.Start();

            if (cell.IsMine)
            {
                _boardGenerator.RelocateMine(_session.Board, row, col, _random);
            }
        }

        if (cell.IsMine)
        {
            cell.Exploded = true;
            cell.Visibility = CellVisibility.Revealed;
            LoseGame();
            return;
        }

        FloodReveal(row, col);
        CheckWinCondition();
    }

    public void ToggleFlag(int row, int col)
    {
        if (_session == null || _session.Status == GameStatus.Won || _session.Status == GameStatus.Lost)
            return;

        var cell = _session.Board.GetCell(row, col);
        if (cell.Visibility == CellVisibility.Revealed)
            return;

        if (_session.Status == GameStatus.NotStarted)
        {
            _session.Status = GameStatus.InProgress;
            _clockService.Start();
        }

        if (cell.Visibility == CellVisibility.Hidden)
        {
            cell.Visibility = CellVisibility.Flagged;
            _session.MinesLeft--;
        }
        else if (cell.Visibility == CellVisibility.Flagged)
        {
            cell.Visibility = CellVisibility.Hidden;
            _session.MinesLeft++;
        }
    }

    public void ChordReveal(int row, int col)
    {
        if (_session == null || _session.Status != GameStatus.InProgress)
            return;

        var cell = _session.Board.GetCell(row, col);
        if (cell.Visibility != CellVisibility.Revealed || cell.NeighborMines == 0)
            return;

        int flagCount = 0;
        var neighbors = GetNeighbors(row, col).ToList();

        foreach (var n in neighbors)
        {
            if (n.Visibility == CellVisibility.Flagged)
            {
                flagCount++;
            }
        }

        if (flagCount == cell.NeighborMines)
        {
            bool hitMine = false;
            foreach (var n in neighbors)
            {
                if (n.Visibility == CellVisibility.Hidden)
                {
                    if (n.IsMine)
                    {
                        n.Exploded = true;
                        n.Visibility = CellVisibility.Revealed;
                        hitMine = true;
                    }
                    else
                    {
                        FloodReveal(n.Row, n.Col);
                    }
                }
            }

            if (hitMine)
            {
                LoseGame();
            }
            else
            {
                CheckWinCondition();
            }
        }
    }

    public GameSessionSnapshot GetSnapshot()
    {
        if (_session == null)
            throw new InvalidOperationException("Game not started.");

        return new GameSessionSnapshot(
            _session.Status,
            _session.Board.GetAllCells().ToList(),
            _session.Board.Rows,
            _session.Board.Cols,
            _session.MinesLeft,
            _clockService.Elapsed
        );
    }

    private void FloodReveal(int startRow, int startCol)
    {
        var queue = new Queue<Cell>();
        var startCell = _session!.Board.GetCell(startRow, startCol);

        if (startCell.Visibility != CellVisibility.Hidden) return;

        queue.Enqueue(startCell);

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (cell.Visibility != CellVisibility.Hidden || cell.IsMine) continue;

            cell.Visibility = CellVisibility.Revealed;

            if (cell.NeighborMines == 0)
            {
                foreach (var n in GetNeighbors(cell.Row, cell.Col))
                {
                    if (n.Visibility == CellVisibility.Hidden && !n.IsMine)
                    {
                        queue.Enqueue(n);
                    }
                }
            }
        }
    }

    private IEnumerable<Cell> GetNeighbors(int row, int col)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int nr = row + i;
                int nc = col + j;

                if (_session!.Board.IsInBounds(nr, nc))
                {
                    yield return _session.Board.GetCell(nr, nc);
                }
            }
        }
    }

    private void LoseGame()
    {
        _session!.Status = GameStatus.Lost;
        _clockService.Stop();

        foreach (var cell in _session.Board.GetAllCells())
        {
            if (cell.IsMine && cell.Visibility != CellVisibility.Flagged)
            {
                cell.Visibility = CellVisibility.Revealed;
            }
        }
    }

    private void CheckWinCondition()
    {
        bool won = true;
        foreach (var cell in _session!.Board.GetAllCells())
        {
            if (!cell.IsMine && cell.Visibility != CellVisibility.Revealed)
            {
                won = false;
                break;
            }
        }

        if (won)
        {
            _session.Status = GameStatus.Won;
            _clockService.Stop();

            foreach (var cell in _session.Board.GetAllCells())
            {
                if (cell.IsMine && cell.Visibility != CellVisibility.Flagged)
                {
                    cell.Visibility = CellVisibility.Flagged;
                    _session.MinesLeft--;
                }
            }
        }
    }
}
