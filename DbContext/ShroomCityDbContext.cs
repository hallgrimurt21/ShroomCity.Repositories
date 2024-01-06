namespace ShroomCity.Repositories.DbContext;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using ShroomCity.Repositories.Seed;

public class ShroomCityDbContext : DbContext
{
    protected ShroomCityDbContext()
    {
    }
    public ShroomCityDbContext(DbContextOptions<ShroomCityDbContext> options) : base(options)
    {

    }

    public DbSet<Attribute> Attributes { get; set; }
    public DbSet<AttributeType> AttributeTypes { get; set; }
    public DbSet<JwtToken> JwtTokens { get; set; }
    public DbSet<Mushroom> Mushrooms { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attribute>()
            .HasKey(a => a.Id);
        // Foreign key to AttributeType
        modelBuilder.Entity<Attribute>()
            .HasOne(a => a.AttributeType)
            .WithOne(at => at.Attribute)
            .HasForeignKey<Attribute>(a => a.AttributeTypeId);
        // Foreign key to User
        modelBuilder.Entity<Attribute>()
            .HasOne(a => a.RegisteredBy)
            .WithMany()
            .HasForeignKey(a => a.RegisteredById);

        modelBuilder.Entity<AttributeType>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<User>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<JwtToken>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Mushroom>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Permission>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Role>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<User>()
            .HasKey(a => a.Id);

        modelBuilder.Seed();
    }
}


