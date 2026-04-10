namespace Minesweeper.Core.Interfaces;
using System;

public interface IClockService
{
    TimeSpan Elapsed { get; }
    void Start();
    void Stop();
    void Reset();
}
