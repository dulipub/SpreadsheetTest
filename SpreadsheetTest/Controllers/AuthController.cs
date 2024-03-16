using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpreadsheetTest.Models;
using SpreadsheetTest.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypto = BCrypt.Net.BCrypt;

namespace SpreadsheetTest.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration config, AppDbContext context)
    {
        _context = context;
        _configuration = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUser newUser)
    {
        if (!ValidateRegistration(newUser))
        {
            return BadRequest("User with email already exists");
        }

        var user = new User
        {
            UserName = newUser.UserName.ToLower(),
            Email = newUser.Email.ToLower(),
            Validated = false
        };

        user.Password = BCrypto.HashPassword(newUser.Password);
        user.Salt = string.Empty;

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUser login)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == login.Email.ToLower());
        if (user == null)
        {
            return BadRequest("No user found");
        }

        var valid = BCrypto.Verify(login.Password, user.Password);
        if (!valid)
        {
            return BadRequest("Login Falied");
        }

        var token = CreateToken(user);
        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Application:Token").Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
                claims:claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials:credentials
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    private bool ValidateRegistration(RegisterUser newUser)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == newUser.Email.ToLower());
        if (existingUser != null && existingUser.Validated)
        {
            return false;
        }

        return true;
    }
}
