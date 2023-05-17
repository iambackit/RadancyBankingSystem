using RadancyBankingSystem.Models;

namespace RadancyBankingSystem.Services;

internal class UserRepository : IUserRepository
{
    public UserDto Add()
    {
        var user = new UserDto { Id = UserDataStore.AccountId++ };
        UserDataStore.Current.Users.Add(user);
        return user;
    }

    public UserDto? Get(int id)
    {
        return UserDataStore.Current.Users.Find(x => x.Id == id);
    }

    public IEnumerable<UserDto> Get()
    {
        return UserDataStore.Current.Users;
    }
}