namespace ShroomCity.Repositories.Implementations;

using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.Entities;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Exceptions;
using ShroomCity.Models.Constants;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Utilities.Hasher;

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
        var salt = Hasher.GenerateSalt();
        var hashedPassword = Hasher.HashPassword(inputModel.Password, salt);
        var newUser = new User
        {
            Name = inputModel.FullName,
            EmailAddress = inputModel.EmailAddress,
            HashedPassword = hashedPassword,
            Bio = inputModel.Bio,
            Role = analystRole,
            RegisterationDate = DateTime.Now.ToUniversalTime(),
        };
        _ = this.context.Users.Add(newUser);
        _ = await this.context.SaveChangesAsync();
        return new UserDto
        {
            Name = newUser.Name,
            EmailAddress = newUser.EmailAddress,
            Permissions = permissions,
            TokenId = token
        };
    }

    public async Task<UserDto?> SignIn(LoginInputModel inputModel)
    {
        var user = await this.context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress) ?? throw new UserNotFoundException(inputModel.EmailAddress);
        var passwordParts = user.HashedPassword.Split('.');
        var salt = passwordParts[0];
        var hashedPassword = Hasher.HashPassword(inputModel.Password, salt);
        var token = await this.tokenRepository.CreateToken();
        var permissions = user.Role.Permissions.Select(p => p.Code).ToList();
        if (hashedPassword != user.HashedPassword)
        {
            return null;
        }
        return new UserDto
        {
            Name = user.Name,
            EmailAddress = user.EmailAddress,
            Permissions = permissions,
            TokenId = token
        };
    }
}
