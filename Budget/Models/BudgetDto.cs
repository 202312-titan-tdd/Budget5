namespace Budget.Models;

public class BudgetDto
{
    public int Amount { get; set; }
    public string YearMonth { get; set; }

    public decimal DailyAmount()
    {
        return (decimal)Amount / Days();
    }

    private int Days()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }
}