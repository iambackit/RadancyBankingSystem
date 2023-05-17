using RadancyBankingSystem.Models;

namespace RadancyBankingSystem.Services;

public interface IUserRepository
{
    public UserDto Add();
    public UserDto? Get(int id);
    public IEnumerable<UserDto> Get();
}