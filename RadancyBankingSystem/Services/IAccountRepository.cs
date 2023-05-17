using RadancyBankingSystem.Models;

namespace RadancyBankingSystem.Services;

public interface IAccountRepository
{
    void Add(UserDto  user, AccountDtoForCreation account);

    void Remove(UserDto userDto, AccountDto account);
    AccountDto? Get(UserDto user, int accountId);
    void Deposit(AccountDto  account, AccountDtoForDeposit deposit);
    decimal GetBalanceForAllAccount(UserDto user);
    bool MinimumThresholdNotReached(AccountDto account, AccountDtoForWithdraw withdraw);
    void Withdraw(AccountDto account, AccountDtoForWithdraw withdraw);
}