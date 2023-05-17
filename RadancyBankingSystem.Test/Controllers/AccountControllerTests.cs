using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RadancyBankingSystem.Controllers;
using RadancyBankingSystem.Models;
using RadancyBankingSystem.Services;

namespace RadancyBankingSystem.Test.Controllers;

[TestFixture]
public class AccountControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _accountController = new AccountController(_userRepositoryMock.Object, _accountRepositoryMock.Object);
    }

    private AccountController _accountController;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IAccountRepository> _accountRepositoryMock;

    [TestCase(0)]
    [TestCase(50)]
    public void CreateAccount_BalanceIsSmallerThan100_ReturnsBadRequest(decimal amount)
    {
        var result = _accountController.CreateAccount(default, new AccountDtoForCreation
        {
            Balance = amount
        });

        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should()
            .Be("An account cannot have less than $100");
    }

    [TestCase(1)]
    [TestCase(2)]
    public void CreateAccount_UserIsNotFound_ReturnsBadRequest(int userId)
    {
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns((UserDto?)null);

        var result = _accountController.CreateAccount(userId, new AccountDtoForCreation
        {
            Balance = 500
        });

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("User is not found");
    }

    [TestCase(1)]
    [TestCase(2)]
    public void CreateAccount_UserCreatedSuccessfully(int userId)
    {
        var expectedUser = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(expectedUser);

        var accountDtoForCreation = new AccountDtoForCreation
        {
            Balance = 500
        };
        var result = _accountController.CreateAccount(userId, accountDtoForCreation);

        _accountRepositoryMock.Verify(repository => repository.Add(expectedUser, accountDtoForCreation), Times.Once);
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should()
            .Be(expectedUser);
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void DeleteAccount_UserIsNotFound_ReturnsBadRequest(int userId, int accountId)
    {
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns((UserDto?)null);

        var result = _accountController.DeleteAccount(userId, accountId);

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("User is not found");
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void DeleteAccount_AccountIsNotFound_ReturnsBadRequest(int userId, int accountId)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns((AccountDto?)null);

        var result = _accountController.DeleteAccount(userId, accountId);

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("Account is not found");
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void DeleteAccount_AccountSuccessfullyDeleted(int userId, int accountId)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        var accountDto = new AccountDto();
        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns(accountDto);

        var result = _accountController.DeleteAccount(userId, accountId);

        _accountRepositoryMock.Verify(repository => repository.Remove(userDto, accountDto), Times.Once);
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should()
            .Be(userDto);
    }

    [TestCase(15000)]
    [TestCase(500000)]
    public void Deposit_AmountIsBiggerThan10000_BadRequest(decimal amount)
    {
        var result = _accountController.Deposit(default, default, new AccountDtoForDeposit { Amount = amount });

        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should()
            .Be("Deposit amount cannot be bigger than $10000");
    }

    [TestCase(1)]
    [TestCase(2)]
    public void Deposit_UserIsNotFound_ReturnsBadRequest(int userId)
    {
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns((UserDto?)null);

        var result = _accountController.Deposit(userId, default, new AccountDtoForDeposit());

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("User is not found");
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void Deposit_AccountIsNotFound_ReturnsBadRequest(int userId, int accountId)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns((AccountDto?)null);

        var result = _accountController.Deposit(userId, accountId, new AccountDtoForDeposit());

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("Account is not found");
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void Deposit_SuccessfulDeposit(int userId, int accountId)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        var accountDto = new AccountDto();
        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns(accountDto);

        var accountDtoForDeposit = new AccountDtoForDeposit();
        var result = _accountController.Deposit(userId, accountId, accountDtoForDeposit);

        _accountRepositoryMock.Verify(repository => repository.Deposit(accountDto, accountDtoForDeposit), Times.Once);
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should()
            .Be(userDto);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void Withdraw_UserIsNotFound_ReturnsBadRequest(int userId)
    {
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns((UserDto?)null);

        var result = _accountController.Withdraw(userId, default, new AccountDtoForWithdraw());

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("User is not found");
    }

    [TestCase(1, 100, 10)]
    [TestCase(2, 12345, 100)]
    public void Withdraw_AmountIsBiggerThan90Percent_ReturnsBadRequest(int userId, decimal amountToDeposit,
        decimal allAccountBalance)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.GetBalanceForAllAccount(userDto))
            .Returns(allAccountBalance);

        var accountDtoForWithdraw = new AccountDtoForWithdraw
        {
            Amount = amountToDeposit
        };
        var result = _accountController.Withdraw(userId, default, accountDtoForWithdraw);

        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should()
            .Be("A user cannot withdraw more than 90% of their total balance from an account in a single transaction");
    }

    [TestCase(1, 2)]
    [TestCase(2, 3)]
    public void Withdraw_AccountIsNotFound_ReturnsBadRequest(int userId, int accountId)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns((AccountDto?)null);

        var result = _accountController.Withdraw(userId, accountId, new AccountDtoForWithdraw());

        result.Should().BeOfType<NotFoundObjectResult>().Which.Value.Should()
            .Be("Account is not found");
    }

    [TestCase(1, 2, 50)]
    [TestCase(2, 3, 100)]
    public void WithDraw_AccountMinimumThresholdNotReached_BadRequest(int userId, int accountId, decimal withdrawAmount)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.GetBalanceForAllAccount(userDto))
            .Returns(10000);

        var accountDto = new AccountDto();
        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns(accountDto);
        var accountDtoForWithdraw = new AccountDtoForWithdraw
        {
            Amount = withdrawAmount
        };
        _accountRepositoryMock
            .Setup(repository => repository.MinimumThresholdNotReached(accountDto, accountDtoForWithdraw))
            .Returns(true);

        var result = _accountController.Withdraw(userId, accountId, accountDtoForWithdraw);

        result.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should()
            .Be("An account cannot have less than $100");
    }

    [TestCase(1, 2, 50)]
    [TestCase(2, 3, 100)]
    public void WithDraw_SuccessfulWithdrawing(int userId, int accountId, decimal withdrawAmount)
    {
        var userDto = new UserDto();
        _userRepositoryMock
            .Setup(repository => repository.Get(userId))
            .Returns(userDto);

        _accountRepositoryMock
            .Setup(repository => repository.GetBalanceForAllAccount(userDto))
            .Returns(10000);

        var accountDto = new AccountDto();
        _accountRepositoryMock
            .Setup(repository => repository.Get(userDto, accountId))
            .Returns(accountDto);
        var accountDtoForWithdraw = new AccountDtoForWithdraw
        {
            Amount = withdrawAmount
        };
        _accountRepositoryMock
            .Setup(repository => repository.MinimumThresholdNotReached(accountDto, accountDtoForWithdraw))
            .Returns(false);

        var result = _accountController.Withdraw(userId, accountId, accountDtoForWithdraw);

        _accountRepositoryMock.Verify(repository => repository.Withdraw(accountDto, accountDtoForWithdraw));
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should()
            .Be(userDto);
    }
}