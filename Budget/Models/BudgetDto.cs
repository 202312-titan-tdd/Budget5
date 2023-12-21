namespace Budget.Models;

public class BudgetDto
{
    public int Amount { get; set; }
    public string YearMonth { get; set; }

    public decimal DailyAmount()
    {
        return (decimal)Amount / Days();
    }

    public DateTime FirstDay()
    {
        return DateTime.ParseExact(YearMonth, "yyyyMM", null);
    }

    public DateTime LastDay()
    {
        return DateTime.ParseExact(YearMonth + Days(), "yyyyMMdd", null);
    }

    private int Days()
    {
        var firstDay = FirstDay();
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }

    public Period CreatePeriod()
    {
        return new Period(FirstDay(), LastDay());
    }

    public decimal OverlappingAmount(Period period)
    {
        return DailyAmount() * period.OverlappingDays(CreatePeriod());
    }
}