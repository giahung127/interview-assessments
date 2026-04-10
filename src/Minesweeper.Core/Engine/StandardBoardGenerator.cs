namespace Minesweeper.Core.Engine;

using System;
using System.Collections.Generic;
using Minesweeper.Core.Interfaces;
using Minesweeper.Core.Models;

public class StandardBoardGenerator : IBoardGenerator
{
    public Board Generate(int rows, int cols, int mineCount, IRandomProvider random)
    {
        var board = new Board(rows, cols, mineCount);
        int minesPlaced = 0;

        while (minesPlaced < mineCount)
        {
            int r = random.Next(rows);
            int c = random.Next(cols);

            var cell = board.GetCell(r, c);
            if (!cell.IsMine)
            {
                cell.IsMine = true;
                minesPlaced++;
            }
        }

        ComputeNeighborMines(board);
        return board;
    }

    public void RelocateMine(Board board, int row, int col, IRandomProvider random)
    {
        var cell = board.GetCell(row, col);
        if (!cell.IsMine) return;

        cell.IsMine = false;
        
        while (true)
        {
            int r = random.Next(board.Rows);
            int c = random.Next(board.Cols);
            
            if ((r != row || c != col) && !board.GetCell(r, c).IsMine)
            {
                board.GetCell(r, c).IsMine = true;
                break;
            }
        }

        ComputeNeighborMines(board);
    }

    public void ComputeNeighborMines(Board board)
    {
        for (int r = 0; r < board.Rows; r++)
        {
            for (int c = 0; c < board.Cols; c++)
            {
                var cell = board.GetCell(r, c);
                if (cell.IsMine)
                {
                    cell.NeighborMines = 0;
                    continue;
                }

                int count = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        int nr = r + i;
                        int nc = c + j;
                        
                        if (board.IsInBounds(nr, nc) && board.GetCell(nr, nc).IsMine)
                        {
                            count++;
                        }
                    }
                }
                cell.NeighborMines = count;
            }
        }
    }
}
