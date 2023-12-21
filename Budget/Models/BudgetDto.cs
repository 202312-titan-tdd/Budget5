namespace Budget.Models;

public class BudgetDto
{
    public string YearMonth { get; set; }
    public int Amount { get; set; }

    public int Days()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM",null);
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }
}