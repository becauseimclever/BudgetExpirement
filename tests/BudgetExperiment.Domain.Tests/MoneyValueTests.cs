using BudgetExperiment.Domain;

namespace BudgetExperiment.Domain.Tests;

public class MoneyValueTests
{
    [Fact]
    public void Create_Should_Normalize_Currency_And_Round()
    {
        var money = MoneyValue.Create("usd", 10.123m);
        Assert.Equal("USD", money.Currency);
        Assert.Equal(10.12m, money.Amount); // rounded away from zero
    }

    [Fact]
    public void Add_Same_Currency_Sums_Amounts()
    {
        var a = MoneyValue.Create("USD", 5m);
        var b = MoneyValue.Create("usd", 7.25m);
        var result = a + b;
        Assert.Equal("USD", result.Currency);
        Assert.Equal(12.25m, result.Amount);
    }

    [Fact]
    public void Add_Different_Currency_Throws()
    {
        var a = MoneyValue.Create("USD", 5m);
        var b = MoneyValue.Create("EUR", 5m);
        var ex = Assert.Throws<DomainException>(() => _ = a + b);
        Assert.Contains("different currencies", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Subtract_Same_Currency_Calculates_Difference()
    {
        var a = MoneyValue.Create("USD", 10m);
        var b = MoneyValue.Create("USD", 3.25m);
        var result = a - b;
        Assert.Equal("USD", result.Currency);
        Assert.Equal(6.75m, result.Amount);
    }

    [Fact]
    public void Subtract_Different_Currency_Throws()
    {
        var a = MoneyValue.Create("USD", 5m);
        var b = MoneyValue.Create("EUR", 5m);
        var ex = Assert.Throws<DomainException>(() => _ = a - b);
        Assert.Contains("different currencies", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_Should_Allow_Negative_Amounts()
    {
        var money = MoneyValue.Create("USD", -10.50m);
        Assert.Equal("USD", money.Currency);
        Assert.Equal(-10.50m, money.Amount);
    }

    [Fact]
    public void Create_Should_Allow_Zero_Amount()
    {
        var money = MoneyValue.Create("USD", 0m);
        Assert.Equal("USD", money.Currency);
        Assert.Equal(0m, money.Amount);
    }

    [Fact]
    public void Abs_Should_Return_Absolute_Value()
    {
        var negative = MoneyValue.Create("USD", -10.50m);
        var positive = negative.Abs();
        Assert.Equal("USD", positive.Currency);
        Assert.Equal(10.50m, positive.Amount);

        var alreadyPositive = MoneyValue.Create("USD", 15.25m);
        var stillPositive = alreadyPositive.Abs();
        Assert.Equal("USD", stillPositive.Currency);
        Assert.Equal(15.25m, stillPositive.Amount);
    }

    [Fact]
    public void Negate_Should_Return_Negated_Value()
    {
        var positive = MoneyValue.Create("USD", 10.50m);
        var negative = positive.Negate();
        Assert.Equal("USD", negative.Currency);
        Assert.Equal(-10.50m, negative.Amount);

        var originallyNegative = MoneyValue.Create("USD", -5.25m);
        var nowPositive = originallyNegative.Negate();
        Assert.Equal("USD", nowPositive.Currency);
        Assert.Equal(5.25m, nowPositive.Amount);
    }
}
