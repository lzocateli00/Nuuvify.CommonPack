using Nuuvify.CommonPack.Security.Abstraction;

namespace Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;

public interface IJwtStore
{

    void Set(string username, CredentialToken tokenResult, string cacheType);
    Task<CredentialToken> Get(string username, string cacheType, CancellationToken cancellationToken = default);
    Task Clear(string username, string cacheType, CancellationToken cancellationToken = default);
    Task ClearAll(string cacheType, CancellationToken cancellationToken = default);
}

