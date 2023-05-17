using Microsoft.AspNetCore.Mvc;
using RadancyBankingSystem.Models;
using RadancyBankingSystem.Services;

namespace RadancyBankingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;

    public AccountController(IUserRepository userRepository, IAccountRepository accountRepository)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
    }

    [HttpPost("CreateAccount")]
    public IActionResult CreateAccount(int userId, AccountDtoForCreation account)
    {
        if (account.Balance < 100)
            return BadRequest("An account cannot have less than $100");

        var user = _userRepository.Get(userId);
        if (user is null)
            return NotFound("User is not found");

        _accountRepository.Add(user, account);

        return Ok(user);
    }

    [HttpDelete("DeleteAccount")]
    public IActionResult DeleteAccount(int userId, int accountId)
    {
        var user = _userRepository.Get(userId);
        if (user is null)
            return NotFound("User is not found");

        var account = _accountRepository.Get(user, accountId);
        if (account is null)
            return NotFound("Account is not found");

        _accountRepository.Remove(user, account);

        return Ok(user);
    }

    [HttpPost("Deposit")]
    public IActionResult Deposit(int userId, int accountId, AccountDtoForDeposit deposit)
    {
        if (deposit.Amount > 10000)
            return BadRequest("Deposit amount cannot be bigger than $10000");

        var user = _userRepository.Get(userId);
        if (user is null)
            return NotFound("User is not found");

        var account = _accountRepository.Get(user, accountId);
        if (account is null)
            return NotFound("Account is not found");

        _accountRepository.Deposit(account, deposit);

        return Ok(user);
    }

    [HttpPost("Withdraw")]
    public IActionResult Withdraw(int userId, int accountId, AccountDtoForWithdraw withdraw)
    {
        var user = _userRepository.Get(userId);
        if (user is null)
            return NotFound("User is not found");

        if (withdraw.Amount > _accountRepository.GetBalanceForAllAccount(user) * 0.9m)
            return BadRequest(
                "A user cannot withdraw more than 90% of their total balance from an account in a single transaction");

        var account = _accountRepository.Get(user, accountId);
        if (account is null)
            return NotFound("Account is not found");

        if (_accountRepository.MinimumThresholdNotReached(account, withdraw))
            return BadRequest("An account cannot have less than $100");
        
        _accountRepository.Withdraw(account, withdraw);

        return Ok(user);
    }
}