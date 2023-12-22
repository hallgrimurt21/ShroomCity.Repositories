namespace ShroomCity.Repositories.Implementations;

using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Exceptions;
public class AccountRepository : IAccountRepository
{
    private readonly ShroomCityDbContext context;

    public AccountRepository(ShroomCityDbContext context) => this.context = context;

    public async Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        var existingUser = await this.context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
        if (existingUser != null)
        {
            return null;
        }
        var analystRole = await this.context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Analyst") ?? throw new RoleNotFoundException("Analyst");

        var permissions = analystRole.Permissions.Select(p => p.Code).ToList();
        var token = 123; /// TODO
        var newUser = new User
        {
            Name = inputModel.FullName,
            EmailAddress = inputModel.EmailAddress,
            HashedPassword = inputModel.Password,
            Bio = inputModel.Bio,
            Role = analystRole,
            RegisterationDate = DateTime.Now
        };
        this.context.Users.Add(newUser);
        await this.context.SaveChangesAsync();
        return new UserDto { Name = newUser.Name, EmailAddress = newUser.EmailAddress, Permissions = permissions, TokenId = token };
    }

    public Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}
