namespace ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.Interfaces;

public class TokenRepository : ITokenRepository
{
    public Task BlacklistToken(int tokenId)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateToken()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenBlacklisted(int tokenId)
    {
        throw new NotImplementedException();
    }
}
