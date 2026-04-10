using System;
using System.Collections.Generic;

namespace Minesweeper.Core.Models;

public record GameSessionSnapshot(
    GameStatus Status,
    IReadOnlyList<Cell> Cells,
    int Rows,
    int Cols,
    int MinesLeft,
    TimeSpan ElapsedTime
);
