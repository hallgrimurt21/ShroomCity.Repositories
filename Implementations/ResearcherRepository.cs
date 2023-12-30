namespace ShroomCity.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Repositories.Interfaces;

public class ResearcherRepository : IResearcherRepository
{
    private readonly ShroomCityDbContext context;

    public ResearcherRepository(ShroomCityDbContext context) => this.context = context;
    public async Task<int?> CreateResearcher(string createdBy, ResearcherInputModel inputModel)
    {
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
        if (user == null)
        {
            return null;
        }

        var researcherRole = await this.context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.Researcher);
        if (researcherRole == null)
        {
            return null;
        }

        user.Role = researcherRole;

        await this.context.SaveChangesAsync();

        return user.Id;
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

    public async Task<ResearcherDto?> GetResearcherByEmailAddress(string emailAddress)
    {
        var user = await this.context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.EmailAddress == emailAddress && (u.Role.Name == RoleConstants.Researcher || u.Role.Name == RoleConstants.Admin));

        if (user == null)
        {
            return null;
        }

        var mushrooms = await this.context.Attributes
            .Include(a => a.Mushrooms)
            .Where(a => a.RegisteredById == user.Id)
            .SelectMany(a => a.Mushrooms)
            .Distinct()
            .ToListAsync();

        var researcher = new ResearcherDto
        {
            Id = user.Id,
            Name = user.Name,
            EmailAddress = user.EmailAddress,
            Bio = user.Bio,
            AssociatedMushrooms = mushrooms.Select(m => new MushroomDto { Name = m.Name, Description = m.Description }).ToList()
        };

        return researcher;
    }

    public async Task<ResearcherDto?> GetResearcherById(int id)
    {
        var user = await this.context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Id == id && (u.Role.Name == RoleConstants.Researcher || u.Role.Name == RoleConstants.Admin));

        if (user == null)
        {
            return null;
        }

        var mushrooms = await this.context.Attributes
        .Include(a => a.Mushrooms)
        .Where(a => a.RegisteredById == user.Id)
        .SelectMany(a => a.Mushrooms)
        .Distinct()
        .ToListAsync();

        var researcher = new ResearcherDto
        {
            Id = user.Id,
            Name = user.Name,
            EmailAddress = user.EmailAddress,
            Bio = user.Bio,
            AssociatedMushrooms = mushrooms.Select(m => new MushroomDto { Name = m.Name, Description = m.Description }).ToList()
        };

        return researcher;
    }
}
