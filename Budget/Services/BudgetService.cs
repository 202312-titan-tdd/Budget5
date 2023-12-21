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
            var totalAmount = 0m;
            foreach (var budgetDto in budgetDtos)
            {
                DateTime overlappingEnd;
                DateTime overlappingStart;
                if (budgetDto.YearMonth == start.ToString("yyyyMM"))
                {
                    overlappingEnd = budgetDto.LastDay();
                    overlappingStart = start;
                    // var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
                    // totalAmount += budgetDto.DailyAmount() * overlappingDays;
                }
                else if (budgetDto.YearMonth == end.ToString("yyyyMM"))
                {
                    overlappingEnd = end;
                    overlappingStart = budgetDto.FirstDay();
                    // var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
                    // totalAmount += budgetDto.DailyAmount() * overlappingDays;
                }
                else
                {
                    overlappingEnd = budgetDto.LastDay();
                    overlappingStart = budgetDto.FirstDay();
                    // var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
                    // totalAmount += budgetDto.DailyAmount() * overlappingDays;
                }

                var overlappingDays = (overlappingEnd - overlappingStart).Days + 1;
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