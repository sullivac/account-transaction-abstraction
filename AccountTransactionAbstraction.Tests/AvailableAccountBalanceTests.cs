namespace AccountTransactionAbstraction.Tests;

using AccountTransactionAbstraction;

public class AvailableAccountBalanceTests
{
    [Theory]
    [InlineData(CreditDebit.Credit)]
    [InlineData(CreditDebit.Debit)]
    public void
        ApplyToBalance_WhenCompletedBalance_ThrowsInvalidOperationException(
            CreditDebit creditDebit)
    {
        var sut = new AvailableAccountBalance("1", 100);

        var transaction =
            new Wire(1, "1", 100, creditDebit, WireStatus.Completed);

        Action act = () => sut.ApplyToBalance(transaction);

        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(CreditDebit.Credit, 200)]
    [InlineData(CreditDebit.Debit, 0)]
    public void ApplyToBalance_WhenCompletedWire_UpdatesBalance(
        CreditDebit creditDebit,
        decimal expectedBalance)
    {
        var sut = new AvailableAccountBalance("1", 100);

        var transaction =
            new Wire(1, "1", 100, creditDebit, WireStatus.Pending);

        var result = sut.ApplyToBalance(transaction);

        result.Balance.Should().Be(expectedBalance);
    }

    [Theory]
    [InlineData(CreditDebit.Credit, 200)]
    [InlineData(CreditDebit.Debit, 0)]
    public void ApplyToBalance_WhenCashTransaction_UpdatesBalance(
        CreditDebit creditDebit,
        decimal expectedBalance)
    { 
        var sut = new AvailableAccountBalance("1", 100);

        var transaction =
            new CashTransaction(1, "1", 100, creditDebit);

        var result = sut.ApplyToBalance(transaction);

        result.Balance.Should().Be(expectedBalance);
    }
}