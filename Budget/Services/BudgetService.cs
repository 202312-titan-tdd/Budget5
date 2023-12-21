#region

using Budget.Interfaces;
using Budget.Models;

#endregion

namespace Budget.Services;

public class Period
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime End { get; private set; }
    public DateTime Start { get; private set; }

    public int OverlappingDays(BudgetDto budgetDto)
    {
        DateTime overlappingEnd;
        DateTime overlappingStart;
        if (budgetDto.YearMonth == Start.ToString("yyyyMM"))
        {
            overlappingEnd = budgetDto.LastDay();
            overlappingStart = Start;
        }
        else if (budgetDto.YearMonth == End.ToString("yyyyMM"))
        {
            overlappingEnd = End;
            overlappingStart = budgetDto.FirstDay();
        }
        else
        {
            overlappingEnd = budgetDto.LastDay();
            overlappingStart = budgetDto.FirstDay();
        }

        var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
        return overlappingDays;
    }
}

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }

        var budgetDtos = _budgetRepo.GetAll();

        var budgetDomainModel = new BudgetDomainModel(budgetDtos);
        if (start.Month != end.Month)
        {
            var totalAmount = 0m;
            foreach (var budgetDto in budgetDtos)
            {
                var overlappingDays = new Period(start, end).OverlappingDays(budgetDto);
                totalAmount += budgetDto.DailyAmount() * overlappingDays;
            }

            return totalAmount;
        }

        return budgetDomainModel.GetOverlappingAmount(start, end, budgetDtos);
    }
}

public class BudgetDomainModel
{
    private readonly List<BudgetDto> _budgetDtos;

    public BudgetDomainModel(List<BudgetDto> budgetDtos)
    {
        _budgetDtos = budgetDtos;
    }

    public decimal GetOverlappingAmount(DateTime overlappingStart, DateTime overlappingEnd, List<BudgetDto> budgetDtos)
    {
        var budgetDto = budgetDtos.FirstOrDefault(x => x.YearMonth == overlappingStart.ToString("yyyyMM"));
        if (budgetDto == null)
        {
            return 0;
        }

        var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
        return budgetDto.DailyAmount() * overlappingDays;
    }
}