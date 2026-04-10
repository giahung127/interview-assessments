using Minesweeper.Core.Interfaces;

namespace Minesweeper.App.Services;

public sealed class LocalDateDailyChallengeService : IDailyChallengeService
{
    public int GetDailySeed(DateOnly localDate)
    {
        // Stable seed across launches for the same local date.
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + localDate.Year;
            hash = (hash * 31) + localDate.Month;
            hash = (hash * 31) + localDate.Day;
            hash = (hash * 31) + 7919; // Salt for spread.

            if (hash == int.MinValue)
            {
                return int.MaxValue;
            }

            return Math.Abs(hash);
        }
    }
}
