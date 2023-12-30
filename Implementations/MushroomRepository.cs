namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Repositories.DbContext;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;

public class MushroomRepository : IMushroomRepository
{
    private readonly ShroomCityDbContext context;
    public MushroomRepository(ShroomCityDbContext context) => this.context = context;
    public async Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributes)
    {
        // TODO
        // var researcher = await this.context.Users.FirstOrDefaultAsync(u => u.EmailAddress == researcherEmailAddress) ?? throw new ArgumentException("Researcher with the provided email address does not exist.");

        var newMushroom = new Mushroom
        {
            Name = mushroom.Name,
            Description = mushroom.Description,
        };

        if (attributes != null)
        {
            foreach (var attributeDto in attributes)
            {
                var attribute = await this.context.Attributes.FindAsync(attributeDto.Id);
                if (attribute != null)
                {
                    newMushroom.Attributes.Add(attribute);
                }
            }
        }

        this.context.Mushrooms.Add(newMushroom);
        await this.context.SaveChangesAsync();

        return newMushroom.Id;
    }

    public Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteMushroomById(int mushroomId)
    {
        var mushroom = await this.context.Mushrooms.FindAsync(mushroomId);
        if (mushroom == null)
        {
            return false;
        }

        this.context.Mushrooms.Remove(mushroom);
        var saveResult = await this.context.SaveChangesAsync();

        return saveResult > 0;
    }

    public async Task<MushroomDetailsDto?> GetMushroomById(int id)
    {
        var mushroom = await this.context.Mushrooms
        .Include(m => m.Attributes)
        .FirstOrDefaultAsync(m => m.Id == id);

        if (mushroom == null)
        {
            return null;
        }

        var mushroomDetailsDto = new MushroomDetailsDto
        {
            Id = mushroom.Id,
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = mushroom.Attributes.Select(a => new AttributeDto
            {
                Id = a.Id,
                Value = a.Value,
                Type = a.AttributeType.Type,
                RegisteredBy = a.RegisteredBy.Name,
            }).ToList()
        };

        return mushroomDetailsDto;
    }

    public (int totalPages, IEnumerable<MushroomDto> mushrooms) GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateMushroomById(int mushroomId, MushroomUpdateInputModel inputModel)
    {
        var mushroom = await this.context.Mushrooms.FindAsync(mushroomId);
        if (mushroom == null)
        {
            return false;
        }
        mushroom.Name = inputModel.Name;
        if (inputModel.Description != null)
        {
            mushroom.Description = inputModel.Description;
        }
        var saveResult = await this.context.SaveChangesAsync();

        return saveResult > 0;
    }
}
