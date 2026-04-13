namespace Minesweeper.Core.Interfaces;

public interface IClockService
{
    TimeSpan Elapsed { get; }
    void Start();
    void Stop();
    void Reset();
}
