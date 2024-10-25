namespace AccountTransactionAbstraction.Tests;

using AccountTransactionAbstraction;

public class AccountTests
{
    [Theory]
    [InlineData(CreditDebit.Credit)]
    [InlineData(CreditDebit.Debit)]
    public void
        ApplyToBalance_WhenPendingBalance_ThrowsInvalidOperationException(
            CreditDebit creditDebit)
    {
        var sut = new Account("1", 100);

        var transaction =
            new Wire(1, "1", 100, creditDebit, WireStatus.Pending);

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
        var sut = new Account("1", 100);

        var transaction =
            new Wire(1, "1", 100, creditDebit, WireStatus.Completed);

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
        var sut = new Account("1", 100);

        var transaction =
            new CashTransaction(1, "1", 100, creditDebit);

        var result = sut.ApplyToBalance(transaction);

        result.Balance.Should().Be(expectedBalance);
    }
}