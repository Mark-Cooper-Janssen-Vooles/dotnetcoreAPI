using System;
using dotnetCoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetCoreAPI.Data
{
  public class ApplicationDbContext : DbContext
  {
    //options is being passed to our baseclass (DbContext) which came from .EntityFrameworkCore
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<NationalPark> NationalParks { get; set; }
  }
}