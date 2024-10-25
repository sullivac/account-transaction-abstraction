namespace AccountTransactionAbstraction;

public interface IBalanceHolder<TBalanceHolder>
{
    decimal Balance { get; }

    TBalanceHolder ApplyToBalance(ITransaction transaction);
}

public record Account(
    string AccountId,
    decimal Balance) : IBalanceHolder<Account>
{
    public Account ApplyToBalance(ITransaction transaction)
    {
        return this with { Balance = transaction.CalculateBalance(this) };
    }
}

public record AvailableAccountBalance(
    string AccountId,
    decimal Balance) : IBalanceHolder<AvailableAccountBalance>
{
    public AvailableAccountBalance ApplyToBalance(ITransaction transaction)
    {
        return this with { Balance = transaction.CalculateBalance(this) };
    }
}

public interface ITransaction
{
    decimal CalculateBalance(Account account);

    decimal CalculateBalance(AvailableAccountBalance availableAccountBalance);
}

public abstract record Transaction(
    long Id,
    string AccountId,
    decimal Amount,
    CreditDebit CreditDebit)
{
    protected decimal CalculateBalance(decimal balance)
    {
        return CreditDebit switch
        {
            CreditDebit.Credit => balance + Amount,
            CreditDebit.Debit => balance - Amount,
            _ => throw new InvalidOperationException(
                "Invalid credit/debit type.")
        };
    }
}

public record CashTransaction(
    long Id,
    string AccountId,
    decimal Amount,
    CreditDebit CreditDebit)
    : Transaction(
        Id,
        AccountId,
        Amount,
        CreditDebit),
    ITransaction
{
    public decimal CalculateBalance(Account account)
    {
        return CalculateBalance(account.Balance);
    }

    public decimal CalculateBalance(AvailableAccountBalance availableAccountBalance)
    {
        return CalculateBalance(availableAccountBalance.Balance);
    }
}

public record Wire(
    long Id,
    string AccountId,
    decimal Amount,
    CreditDebit CreditDebit,
    WireStatus WireStatus)
    : Transaction(
        Id,
        AccountId,
        Amount,
        CreditDebit),
    ITransaction
{
    public decimal CalculateBalance(Account account)
    {
        if (WireStatus != WireStatus.Completed)
        {
            throw new InvalidOperationException(
                "Cannot calculate balance for account.");
        }

        return CalculateBalance(account.Balance);
    }

    public decimal CalculateBalance(AvailableAccountBalance availableAccountBalance)
    {
        if (WireStatus != WireStatus.Pending)
        {
            throw new InvalidOperationException(
                "Cannot calculate balance for available account balance.");
        }

        return CalculateBalance(availableAccountBalance.Balance);
    }
}

public enum CreditDebit
{
    Credit,
    Debit
}

public enum WireStatus
{
    Pending,
    Completed
}