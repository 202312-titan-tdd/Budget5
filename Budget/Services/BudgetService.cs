﻿using Budget.Interfaces;
using Budget.Models;

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
            var startAmount = budgetDomainModel.GetAmount(start,
                new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)));

            var endAmount = budgetDomainModel.GetAmount(new DateTime(end.Year, end.Month, 1), end);

            var middleMonthAmount = budgetDtos.Where(o => Convert.ToInt32(o.YearMonth) > Convert.ToInt32(start.ToString("yyyyMM"))
                                            && Convert.ToInt32(o.YearMonth) < Convert.ToInt32(end.ToString("yyyyMM"))).Sum(o=>o.Amount);

            return startAmount + endAmount + middleMonthAmount;
        }

        return budgetDomainModel.GetAmount(start, end);
    }
}

public class BudgetDomainModel
{
    private readonly List<BudgetDto> _budgetDtos;

    public BudgetDomainModel(List<BudgetDto> budgetDtos)
    {
        _budgetDtos = budgetDtos;
    }

    public decimal GetAmount(DateTime start, DateTime end)
    {
        var sum = _budgetDtos.Where(x => x.YearMonth == start.ToString("yyyyMM")).Sum(o => o.Amount);

        var daysDiff = (end - start).Days + 1;
        return (decimal) sum / (DateTime.DaysInMonth(start.Year, start.Month)) * daysDiff;
    }
}