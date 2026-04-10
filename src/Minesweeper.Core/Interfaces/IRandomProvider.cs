namespace Minesweeper.Core.Interfaces;

public interface IRandomProvider
{
    int Next(int maxExclusive);
}
