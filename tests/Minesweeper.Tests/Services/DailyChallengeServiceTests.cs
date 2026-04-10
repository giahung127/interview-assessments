using Minesweeper.App.Services;
using Xunit;

namespace Minesweeper.Tests.Services;

public class DailyChallengeServiceTests
{
    [Fact]
    public void GetDailySeed_IsDeterministic_ForSameDate()
    {
        var service = new LocalDateDailyChallengeService();
        var date = new DateOnly(2026, 4, 8);

        var first = service.GetDailySeed(date);
        var second = service.GetDailySeed(date);

        Assert.Equal(first, second);
    }

    [Fact]
    public void GetDailySeed_Changes_AcrossDifferentDates()
    {
        var service = new LocalDateDailyChallengeService();

        var today = service.GetDailySeed(new DateOnly(2026, 4, 8));
        var tomorrow = service.GetDailySeed(new DateOnly(2026, 4, 9));

        Assert.NotEqual(today, tomorrow);
    }
}
