namespace ShroomCity.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models;

{
    public class ShroomCityDbContext : DbContext
{
    public ShroomCityDbContext(DbContextOptions<ShroomCityDbContext> options) : base(options)
    {
    }

    // Define your DbSets here. For example:
    // public DbSet<Shroom> Shrooms { get; set; }
}
}
