namespace ShroomCity.Repositories.Seed;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Entities;
using ShroomCity.Models.Enums;

public static class ModelBuilderExtensions
{
#pragma warning disable IDE1006 // Naming Styles
    private static readonly List<string> _attributeTypes = new()
#pragma warning restore IDE1006 // Naming Styles
    {
        AttributeTypeEnum.Color.ToString(),
        AttributeTypeEnum.Shape.ToString(),
        AttributeTypeEnum.Surface.ToString(),
        AttributeTypeEnum.CapSize.ToString(),
        AttributeTypeEnum.StemSize.ToString()
    };

#pragma warning disable IDE1006 // Naming Styles
    private static readonly List<string> _roles = new()
#pragma warning restore IDE1006 // Naming Styles
    {
        RoleConstants.Admin,
        RoleConstants.Researcher,
        RoleConstants.Analyst,
    };

    public static void Seed(this ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AttributeType>().HasData(
            _attributeTypes.Select((a, idx) => new AttributeType
            {
                Id = idx + 1,
                Type = a
            })
        );

        modelBuilder.Entity<Role>()
            .HasData(_roles.Select((r, idx) => new Role
            {
                Id = idx + 1,
                Name = r
            }));

        modelBuilder.Entity<Permission>()
            .HasData(
                new Permission
                {
                    Id = 1,
                    Code = PermissionConstants.ReadMushrooms,
                    Description = "Allows users to read mushroom data."
                },
                new Permission
                {
                    Id = 2,
                    Code = PermissionConstants.WriteMushrooms,
                    Description = "Allows users to write mushroom data."
                },
                new Permission
                {
                    Id = 3,
                    Code = PermissionConstants.ReadResearchers,
                    Description = "Allows users to read researcher data."
                },
                new Permission
                {
                    Id = 4,
                    Code = PermissionConstants.WriteResearchers,
                    Description = "Allows users to write researcher data."
                }
            );

        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "PermissionRole",
                r => r.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                p => p.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                rp =>
                {
                    rp.HasKey("RoleId", "PermissionId");
                    rp.HasData(
                        // Admin role
                        new { RoleId = 1, PermissionId = 1 },
                        new { RoleId = 1, PermissionId = 2 },
                        new { RoleId = 1, PermissionId = 3 },
                        new { RoleId = 1, PermissionId = 4 },
                        // Researcher role
                        new { RoleId = 2, PermissionId = 1 },
                        new { RoleId = 2, PermissionId = 2 },
                        new { RoleId = 2, PermissionId = 3 },
                        // Analyst role
                        new { RoleId = 3, PermissionId = 1 },
                        new { RoleId = 3, PermissionId = 3 });
                }
            );
    }
}
