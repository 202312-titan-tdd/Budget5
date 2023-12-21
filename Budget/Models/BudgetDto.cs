namespace Budget.Models;

public class BudgetDto
{
    public int Amount { get; set; }
    public string YearMonth { get; set; } = null!;

    public decimal OverlappingAmount(Period period)
    {
        return DailyAmount() * period.OverlappingDays(CreatePeriod());
    }

    private Period CreatePeriod()
    {
        return new Period(FirstDay(), LastDay());
    }

    private decimal DailyAmount()
    {
        return (decimal)Amount / Days();
    }

    private int Days()
    {
        var firstDay = FirstDay();
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }

    private DateTime FirstDay()
    {
        return DateTime.ParseExact(YearMonth, "yyyyMM", null);
    }

    private DateTime LastDay()
    {
        return DateTime.ParseExact(YearMonth + Days(), "yyyyMMdd", null);
    }
}