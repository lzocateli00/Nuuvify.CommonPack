using Nuuvify.CommonPack.Security.JwtCredentials.Model;
using Microsoft.EntityFrameworkCore;

namespace Nuuvify.CommonPack.Security.JwtStore.Ef;


public interface IJwtCacheContext
{
    /// <summary>
    /// A collection of <see cref="T:Nuuvify.CommonPack.Security.JwtCredentials.Model.JwtCacheToken" />
    /// </summary>
    DbSet<JwtCacheToken> Tokens { get; set; }
}

