namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Repositories.DbContext;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using System.Globalization;

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

        _ = this.context.Mushrooms.Add(newMushroom);
        _ = await this.context.SaveChangesAsync();

        return newMushroom.Id;
    }

    public async Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        var researcher = await this.context.Users.FirstOrDefaultAsync(u => u.EmailAddress == researcherEmailAddress) ?? throw new ArgumentException("Researcher with the provided email address does not exist.");

        var mushroom = await this.context.Mushrooms.FindAsync(mushroomId) ?? throw new ArgumentException("Mushroom with the provided ID does not exist.");

        foreach (var entry in inputModel.Entries)
        {
            var attributeType = await this.context.AttributeTypes.FindAsync(entry.Key) ?? throw new ArgumentException($"Attribute type with ID {entry.Key} does not exist.");

            var researchEntry = new Attribute
            {
                Value = entry.Value,
                AttributeType = attributeType,
                RegisteredBy = researcher,
                Mushrooms = new List<Mushroom> { mushroom },
            };

            _ = this.context.Attributes.Add(researchEntry);
        }

        var result = await this.context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> DeleteMushroomById(int mushroomId)
    {
        var mushroom = await this.context.Mushrooms.FindAsync(mushroomId);
        if (mushroom == null)
        {
            return false;
        }

        _ = this.context.Mushrooms.Remove(mushroom);
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

    public async Task<(int totalPages, IEnumerable<MushroomDto> mushrooms)> GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        var query = this.context.Mushrooms.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (stemSizeMinimum.HasValue && stemSizeMaximum.HasValue)
        {
            query = query.Where(m => m.Attributes.Where(a => a.AttributeType.Type == "StemSize").Average(a => double.Parse(a.Value, CultureInfo.InvariantCulture)) >= stemSizeMinimum.Value && m.Attributes.Where(a => a.AttributeType.Type == "StemSize").Average(a => double.Parse(a.Value, CultureInfo.InvariantCulture)) <= stemSizeMaximum.Value);
        }

        if (capSizeMinimum.HasValue && capSizeMaximum.HasValue)
        {
            query = query.Where(m => m.Attributes.Where(a => a.AttributeType.Type == "CapSize").Average(a => double.Parse(a.Value, CultureInfo.InvariantCulture)) >= capSizeMinimum.Value && m.Attributes.Where(a => a.AttributeType.Type == "CapSize").Average(a => double.Parse(a.Value, CultureInfo.InvariantCulture)) <= capSizeMaximum.Value);
        }

        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(m => m.Attributes.Any(a => a.AttributeType.Type == "Color" && a.Value.ToString().Equals(color, StringComparison.OrdinalIgnoreCase)));
        }

        var count = query.Count();
        var totalPages = (int)Math.Ceiling((double)count / pageSize);
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var mushrooms = query.Select(m => new MushroomDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
        }).ToList();
        return (totalPages, mushrooms);
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
