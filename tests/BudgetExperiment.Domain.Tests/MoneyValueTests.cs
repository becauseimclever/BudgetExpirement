namespace BudgetExperiment.Domain.Tests;

using BudgetExperiment.Domain;

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
    public void Negative_Amount_Throws()
    {
        var ex = Assert.Throws<DomainException>(() => MoneyValue.Create("USD", -1m));
        Assert.Contains("negative", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
