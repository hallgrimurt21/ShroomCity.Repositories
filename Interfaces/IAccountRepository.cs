namespace ShroomCity.Repositories.Interfaces;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;

public interface IAccountRepository
{
    Task<UserDto?> Register(RegisterInputModel inputModel);
    Task<UserDto?> SignIn(LoginInputModel inputModel);
}
