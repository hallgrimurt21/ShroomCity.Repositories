namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Models.Constants;
using Microsoft.EntityFrameworkCore;

public class ResearcherRepository : IResearcherRepository
{
    private readonly ShroomCityDbContext context;

    public ResearcherRepository(ShroomCityDbContext context) => this.context = context;
    public Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ResearcherDto>?> GetAllResearchers()
    {

        var users = await this.context.Users
            .Where(u => u.Role.Name == RoleConstants.Researcher || u.Role.Name == RoleConstants.Admin)
            .ToListAsync();

        var researchers = new List<ResearcherDto>();

        foreach (var user in users)
        {
            var researcher = new ResearcherDto
            {
                Id = user.Id,
                Name = user.Name,
                EmailAddress = user.EmailAddress,
                Bio = user.Bio,
                AssociatedMushrooms = await this.context.Mushrooms
                    .Where(m => m.Attributes.Any(a => a.RegisteredById == user.Id))
                    .Select(m => new MushroomDto { Name = m.Name, Description = m.Description })
                    .ToListAsync()
            };

            researchers.Add(researcher);
        }

        return researchers;
    }

    public Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        throw new NotImplementedException();
    }

    public Task<ResearcherDto?> GetResearcherById(int id)
    {
        throw new NotImplementedException();
    }
}
