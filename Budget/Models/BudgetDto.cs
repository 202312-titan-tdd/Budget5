namespace Budget.Models;

public class BudgetDto
{
    public int Amount { get; set; }
    public string YearMonth { get; set; }

    public decimal DailyAmount()
    {
        var daysInMonth = Days();
        var dailyAmount = (decimal)Amount / daysInMonth;
        return dailyAmount;
    }

    public int Days()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
    }
}