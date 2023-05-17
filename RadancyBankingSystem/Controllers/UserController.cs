using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using RadancyBankingSystem.Models;
using RadancyBankingSystem.Services;

namespace RadancyBankingSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("CreateUser")]
    public IActionResult CreateUser()
    {
        var user = _userRepository.Add();
        return Ok(user);
    }

    [HttpGet("Users")]
    public IActionResult GetUsers()
    {
        var users = _userRepository.Get();
        return Ok(users);
    }

    [HttpGet("User")]
    public IActionResult GetUser(int id)
    {
        var user = _userRepository.Get(id);
        if (user is null)
            return NotFound("User is not found");

        return Ok(user);
    }
}