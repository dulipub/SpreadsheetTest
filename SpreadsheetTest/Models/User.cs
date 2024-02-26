using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SpreadsheetTest.Models;

public class User
{
    [Key]
    public int Id { get; set; } //should be in base controler
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Email { get; set; }
    public bool Validated { get; set; }

}
