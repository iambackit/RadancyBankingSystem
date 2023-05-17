using RadancyBankingSystem.Models;

namespace RadancyBankingSystem.Services;

internal class AccountRepository : IAccountRepository
{
    public void Add(UserDto user, AccountDtoForCreation account)
    {
        var userAccount = new AccountDto
        {
            Id = UserDataStore.AccountId++,
            Balance = account.Balance
        };
        user.Accounts.Add(userAccount);
    }

    public void Remove(UserDto user, AccountDto account)
    {
        user.Accounts.Remove(account);
    }

    public AccountDto? Get(UserDto user, int accountId)
    {
        return user.Accounts.Find(dto => dto.Id == accountId);
    }

    public void Deposit(AccountDto account, AccountDtoForDeposit deposit)
    {
        account.Balance += deposit.Amount;
    }

    public decimal GetBalanceForAllAccount(UserDto user)
    {
        return user.Accounts.Sum(dto => dto.Balance);
    }

    public bool MinimumThresholdNotReached(AccountDto account, AccountDtoForWithdraw withdraw)
    {
        return account.Balance - withdraw.Amount < 100;
    }

    public void Withdraw(AccountDto account, AccountDtoForWithdraw withdraw)
    {
        account.Balance -= withdraw.Amount;
    }
}