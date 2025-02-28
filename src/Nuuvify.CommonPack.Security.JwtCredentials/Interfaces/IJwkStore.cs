using Nuuvify.CommonPack.Security.JwtCredentials.Model;

namespace Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;

public interface IJwkStore
{
    void Save(SecurityKeyWithPrivate securityParameteres);
    SecurityKeyWithPrivate GetCurrentKey();
    IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5);
    void Clear();
    bool NeedsUpdate();
    void Update(SecurityKeyWithPrivate securityKeyWithPrivate);
}
