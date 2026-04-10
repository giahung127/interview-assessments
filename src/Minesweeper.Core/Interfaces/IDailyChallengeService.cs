namespace Minesweeper.Core.Interfaces;

public interface IDailyChallengeService
{
    int GetDailySeed(DateOnly localDate);
}
