namespace ShroomCity.Repositories.Implementations;

using ShroomCity.Models.Entities;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Exceptions;

public class TokenRepository : ITokenRepository
{
    private readonly ShroomCityDbContext context;
    public TokenRepository(ShroomCityDbContext context) => this.context = context;
    public async Task BlacklistToken(int tokenId)
    {
        var token = await this.context.JwtTokens.FindAsync(tokenId) ?? throw new TokenNotFoundException(tokenId);

        token.Blacklisted = true;
        _ = await this.context.SaveChangesAsync();
    }

    public async Task<int> CreateToken()
    {
        var token = new JwtToken
        {
            Blacklisted = false
        };
        _ = this.context.JwtTokens.Add(token);
        _ = await this.context.SaveChangesAsync();
        return token.Id;
    }

    public async Task<bool> IsTokenBlacklisted(int tokenId)
    {
        var token = await this.context.JwtTokens.FindAsync(tokenId) ?? throw new TokenNotFoundException(tokenId);
        return token.Blacklisted;
    }
}
