namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Repositories.DbContext;
using Microsoft.EntityFrameworkCore;
using ShroomCity.Models.Entities;
using System.Collections.Generic;
using ShroomCity.Utilities.Exceptions;
using System.Globalization;

public class MushroomRepository : IMushroomRepository
{
    private readonly ShroomCityDbContext context;
    public MushroomRepository(ShroomCityDbContext context) => this.context = context;
    public async Task<int> CreateMushroom(MushroomInputModel mushroom, string researcherEmailAddress, List<AttributeDto> attributes)
    {
        var mushroomAttributes = new List<Attribute>();
        if (attributes != null)
        {
            foreach (var attribute in attributes)
            {
                var researcher = await this.context.Users.FirstOrDefaultAsync(u => u.EmailAddress == attribute.RegisteredBy) ?? throw new ArgumentException("Researcher with the provided email address does not exist.");
                var type = await this.context.AttributeTypes.FirstOrDefaultAsync(at => at.Type == attribute.Type);
                var mushroomAttribute = new Attribute
                {
                    Value = attribute.Value,
                    AttributeType = type,
                    RegisteredBy = researcher
                };
                mushroomAttributes.Add(mushroomAttribute);
            }
        }
        var newMushroom = new Mushroom
        {
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = mushroomAttributes
        };
        _ = this.context.Mushrooms.Add(newMushroom);
        _ = await this.context.SaveChangesAsync();

        return newMushroom.Id;
    }

    public async Task<bool> CreateResearchEntry(int mushroomId, string researcherEmailAddress, ResearchEntryInputModel inputModel)
    {
        var researcher = await this.context.Users.FirstOrDefaultAsync(u => u.EmailAddress == researcherEmailAddress) ?? throw new ResearcherNotFoundException(researcherEmailAddress);

        var mushroom = await this.context.Mushrooms.FindAsync(mushroomId) ?? throw new MushroomNotFoundException();

        foreach (var entry in inputModel.Entries)
        {
            var attributeType = await this.context.AttributeTypes.FirstOrDefaultAsync(u => u.Type == entry.Key) ?? throw new AttributeTypeNotFoundException(entry.Key);

            if (attributeType.Type is "StemSize" or "CapSize")
            {
                try
                { _ = double.Parse(entry.Value, CultureInfo.InvariantCulture); }
                catch { throw new InvalidInputException(entry.Value, "number"); }
            }

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
            .ThenInclude(a => a.AttributeType)
        .Include(m => m.Attributes)
            .ThenInclude(a => a.RegisteredBy) // Assuming RegisteredBy is a User entity
        .FirstOrDefaultAsync(m => m.Id == id);

        if (mushroom == null)
        {
            return null;
        }
        var attributes = new List<AttributeDto>();
        attributes.AddRange(mushroom.Attributes.Select(a => new AttributeDto
        {
            Id = a.Id,
            Value = a.Value,
            Type = a.AttributeType.Type,
            RegisteredBy = a.RegisteredBy.Name,
        }).ToList());

        var mushroomDetailsDto = new MushroomDetailsDto
        {
            Id = mushroom.Id,
            Name = mushroom.Name,
            Description = mushroom.Description,
            Attributes = attributes
        };

        return mushroomDetailsDto;
    }

    public async Task<(int totalPages, IEnumerable<MushroomDto> mushrooms)> GetMushroomsByCriteria(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize, int pageNumber)
    {
        var query = this.context.Mushrooms.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            var lowerCaseName = name.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(lowerCaseName));
        }

        if (stemSizeMinimum.HasValue && stemSizeMaximum.HasValue)
        {
            var stemSizeAttributes = this.context.Attributes
            .Where(a => a.AttributeType.Type == "StemSize")
            .SelectMany(a => a.Mushrooms, (a, m) => new { AttributeValue = a.Value, Mushroom = m })
            .ToList();

            var groupedAttributes = stemSizeAttributes
                .GroupBy(am => am.Mushroom.Id)
                .Select(g => new
                {
                    MushroomId = g.Key,
                    AverageValue = g.Average(am => double.Parse(am.AttributeValue))
                })
                .Where(a => a.AverageValue >= stemSizeMinimum.Value && a.AverageValue <= stemSizeMaximum.Value)
                .ToList();

            var validMushroomIds = groupedAttributes.Select(a => a.MushroomId).ToList();

            query = query.Where(m => validMushroomIds.Contains(m.Id));
        }

        if (capSizeMinimum.HasValue && capSizeMaximum.HasValue)
        {
            var capSizeAttributes = this.context.Attributes
            .Where(a => a.AttributeType.Type == "CapSize")
            .SelectMany(a => a.Mushrooms, (a, m) => new { AttributeValue = a.Value, Mushroom = m })
            .ToList();

            var groupedAttributes = capSizeAttributes
                .GroupBy(am => am.Mushroom.Id)
                .Select(g => new
                {
                    MushroomId = g.Key,
                    AverageValue = g.Average(am => double.Parse(am.AttributeValue))
                })
                .Where(a => a.AverageValue >= capSizeMinimum.Value && a.AverageValue <= capSizeMaximum.Value)
                .ToList();

            var validMushroomIds = groupedAttributes.Select(a => a.MushroomId).ToList();

            query = query.Where(m => validMushroomIds.Contains(m.Id));
        }

        if (!string.IsNullOrEmpty(color))
        {
            query = query.Where(m => m.Attributes.Any(a => a.AttributeType.Type == "Color" && EF.Functions.Like(a.Value, color)));
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
