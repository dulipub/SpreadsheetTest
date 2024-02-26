using System.ComponentModel.DataAnnotations;

namespace SpreadsheetTest.ViewModels;
public class LoginUser
{
    [Required]
    public string Password { get; set; }
    [Required]
    public string Email { get; set; }
}