namespace Budget.Models;

public class Period
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    private DateTime End { get; set; }
    private DateTime Start { get; set; }

    public int OverlappingDays(BudgetDto budgetDto)
    {
        var another = new Period(budgetDto.FirstDay(), budgetDto.LastDay());
        var firstDay = another.Start;
        var lastDay = another.End;
        var overlappingEnd = End < lastDay
            ? End
            : lastDay;
        var overlappingStart = Start > firstDay
            ? Start
            : firstDay;

        return (overlappingEnd - overlappingStart).Days + 1;
    }
}