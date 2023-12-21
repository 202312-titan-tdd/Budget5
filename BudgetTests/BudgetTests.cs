#region

using Budget.Interfaces;
using Budget.Models;
using Budget.Services;
using FluentAssertions;
using NSubstitute;

#endregion

namespace BudgetTests;

[TestFixture]
public class BudgetTests
{
    private IBudgetRepo _budgetRepo = null!;
    private BudgetService _budgetService = null!;

    [SetUp]
    public void Setup()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    [Test]
    public void query_cross_month()
    {
        GivenBudget(
            new BudgetDto()
            {
                YearMonth = "202312",
                Amount = 3100
            },
            new BudgetDto
            {
                YearMonth = "202311",
                Amount = 600
            }
        );

        var start = new DateTime(2023, 11, 01);
        var end = new DateTime(2023, 12, 10);
        var amount = WhenQueryBudget(start, end);

        amount.Should().Be(1600);
    }

    [Test]
    public void query_invalid_period()
    {
        GivenBudget(
            new BudgetDto()
            {
                YearMonth = "202312",
                Amount = 3100
            }
        );

        var amount = WhenQueryBudget(new DateTime(2024, 12, 01), new DateTime(2023, 12, 31));

        amount.Should().Be(0);
    }

    [Test]
    public void query_over_two_month()
    {
        GivenBudget(
            new BudgetDto()
            {
                YearMonth = "202312",
                Amount = 3100
            },
            new BudgetDto()
            {
                YearMonth = "202311",
                Amount = 600
            },
            new BudgetDto()
            {
                YearMonth = "202310",
                Amount = 62000
            }
        );

        var start = new DateTime(2023, 10, 10);
        var end = new DateTime(2023, 12, 10);
        var amount = WhenQueryBudget(start, end);

        amount.Should().Be(45600);
    }

    [Test]
    public void query_partial_month()
    {
        GivenBudget(
            new BudgetDto()
            {
                YearMonth = "202312",
                Amount = 3100
            }
        );

        var start = new DateTime(2023, 12, 01);
        var end = new DateTime(2023, 12, 10);
        var amount = WhenQueryBudget(start, end);

        amount.Should().Be(1000);
    }

    [Test]
    public void query_whole_month()
    {
        GivenBudget(
            new BudgetDto()
            {
                YearMonth = "202312",
                Amount = 3100
            }
        );

        var amount = WhenQueryBudget(new DateTime(2023, 12, 01), new DateTime(2023, 12, 31));

        amount.Should().Be(3100);
    }

    private void GivenBudget(params BudgetDto[] budgetDtos)
    {
        _budgetRepo.GetAll().Returns(budgetDtos.ToList());
    }

    private decimal WhenQueryBudget(DateTime start, DateTime end)
    {
        var amount = _budgetService.Query(start, end);
        return amount;
    }
}