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
                Id = 0
            },
            new()
            {
                Id = 1
            }
        };
    }

    public List<UserDto> Users { get; set; }
    public static UserDataStore Current { get; } = new();
}