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
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return firstDay;
    }

    public DateTime LastDay()
    {
        return DateTime.ParseExact(YearMonth + Days(), "yyyyMMdd", null);
    }

    private int Days()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }
}