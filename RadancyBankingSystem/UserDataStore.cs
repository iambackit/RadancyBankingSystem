using RadancyBankingSystem.Models;

namespace RadancyBankingSystem;

public class UserDataStore
{
    public static int AccountId = 2;

    public UserDataStore()
    {
        Users = new List<UserDto>
        {
            new()
            {
                Id = 0,
                Accounts =
                    new List<AccountDto>
                    {
                        new()
                        {
                            Id = 0,
                            Balance = 150
                        },
                        new()
                        {
                            Id = 1,
                            Balance = 500
                        }
                    }
            },
            new()
            {
                Id = 1,
                Accounts = new List<AccountDto>
                {
                    new()
                    {
                        Id = 2,
                        Balance = 5000
                    }
                }
            }
        };
    }

    public List<UserDto> Users { get; set; }
    public static UserDataStore Current { get; } = new();
}