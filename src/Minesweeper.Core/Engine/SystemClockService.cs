namespace Minesweeper.Core.Engine;

using System.Diagnostics;
using Minesweeper.Core.Interfaces;

public class SystemClockService : IClockService
{
    private readonly Stopwatch _stopwatch = new();

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public void Start() => _stopwatch.Start();

    public void Stop() => _stopwatch.Stop();

    public void Reset() => _stopwatch.Reset();
}
