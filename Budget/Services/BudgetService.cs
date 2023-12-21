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

        var budgetDomainModel = new BudgetDomainModel(budgetDtos);
        if (start.Month != end.Month)
        {
            var startAmount = 0m;
            var startBudgetDto = budgetDtos.FirstOrDefault(x => x.YearMonth == start.ToString("yyyyMM"));
            if (startBudgetDto != null)
            {
                var overlappingDays = (new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)) - start).Days + 1;
                startAmount = startBudgetDto.DailyAmount() * overlappingDays;
            }

            var endAmount = budgetDomainModel.GetOverlappingAmount(new DateTime(end.Year, end.Month, 1), end, budgetDtos);

            var middleMonthAmount = budgetDtos.Where(o => Convert.ToInt32(o.YearMonth) > Convert.ToInt32(start.ToString("yyyyMM")) && Convert.ToInt32(o.YearMonth) < Convert.ToInt32(end.ToString("yyyyMM"))).Sum(o => o.Amount);

            return startAmount + endAmount + middleMonthAmount;
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