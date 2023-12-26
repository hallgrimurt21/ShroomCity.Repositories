namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Utilities.Exceptions;

public class TokenRepository : ITokenRepository
{
    private readonly ShroomCityDbContext context;
    public TokenRepository(ShroomCityDbContext context) => this.context = context;
    public Task BlacklistToken(int tokenId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateToken()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsTokenBlacklisted(int tokenId)
    {
        var token = await this.context.JwtTokens.FindAsync(tokenId) ?? throw new TokenNotFoundException(tokenId);
        return token.Blacklisted;
    }
}
