using Microsoft.EntityFrameworkCore;
using SpreadsheetTest.Models;
using System;

namespace SpreadsheetTest;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}