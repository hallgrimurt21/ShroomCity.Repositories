namespace ShroomCity.Repositories.DbContext;

using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Exceptions;
using ShroomCity.Models.Constants;
public class AccountRepository : IAccountRepository
{
    private readonly ShroomCityDbContext context;
    private readonly ITokenRepository tokenRepository;

    public AccountRepository(ITokenRepository tokenRepository, ShroomCityDbContext context)
    {
        this.tokenRepository = tokenRepository;
        this.context = context;
    }

    public async Task<UserDto?> Register(RegisterInputModel inputModel)
    {
        var existingUser = await this.context.Users
            .FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
        if (existingUser != null)
        {
            return null;
        }
        var analystRole = await this.context.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleConstants.Analyst) ?? throw new RoleNotFoundException(RoleConstants.Analyst);

        var permissions = analystRole.Permissions.Select(p => p.Code).ToList();
        var token = await this.tokenRepository.CreateToken();
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
        return new UserDto { Name = newUser.Name, EmailAddress = newUser.EmailAddress, Permissions = permissions!, TokenId = token };
    }

    public Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}
