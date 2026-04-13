namespace Minesweeper.Core.Engine;

using Minesweeper.Core.Interfaces;

public class DotNetRandomProvider : IRandomProvider
{
    private readonly Random _random;

    public DotNetRandomProvider()
    {
        _random = new Random();
    }

    public DotNetRandomProvider(int seed)
    {
        _random = new Random(seed);
    }

    public int Next(int maxExclusive)
    {
        return _random.Next(maxExclusive);
    }
}
