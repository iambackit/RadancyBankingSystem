namespace RadancyBankingSystem.Models;

public class UserDto
{
    public int Id { get; set; }
    public List<AccountDto> Accounts { get; set; } = new();
}