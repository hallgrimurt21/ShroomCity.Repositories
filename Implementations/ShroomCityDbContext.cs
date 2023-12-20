namespace ShroomCity.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using ShroomCity.Repositories.Seed;

public class ShroomCityDbContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ShroomCityDbContext(DbContextOptions<ShroomCityDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

    public DbSet<Attribute> Attributes { get; set; }
    public DbSet<AttributeType> AttributeTypes { get; set; }
    public DbSet<JwtToken> JwtTokens { get; set; }
    public DbSet<Mushroom> Mushrooms { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
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


