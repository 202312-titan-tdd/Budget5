#region

using Budget.Interfaces;
using Budget.Models;

#endregion

namespace Budget.Services;

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

        var totalAmount = 0m;
        var period = new Period(start, end);
        foreach (var budgetDto in budgetDtos)
        {
            var overlappingDays = period.OverlappingDays(CreatePeriod(budgetDto));
            totalAmount += budgetDto.DailyAmount() * overlappingDays;
        }

        return totalAmount;
    }

    private static Period CreatePeriod(BudgetDto budgetDto)
    {
        return new Period(budgetDto.FirstDay(), budgetDto.LastDay());
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