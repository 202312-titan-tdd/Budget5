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
        var overlappingEnd = End < budgetDto.LastDay()
            ? End
            : budgetDto.LastDay();
        var overlappingStart = Start > budgetDto.FirstDay()
            ? Start
            : budgetDto.FirstDay();

        return (overlappingEnd - overlappingStart).Days + 1;
    }
}