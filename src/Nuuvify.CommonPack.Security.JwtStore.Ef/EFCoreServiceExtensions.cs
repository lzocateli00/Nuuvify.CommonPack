using Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;
using Nuuvify.CommonPack.Security.JwtStore.Ef;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Builder extension methods for registering crypto services
/// </summary>
public static class EFCoreServiceExtensions
{
    /// <summary>
    /// Sets the signing credential.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static IJwksBuilder PersistKeysToDatabaseStore<TContext>(this IJwksBuilder builder)
        where TContext : DbContext, ISecurityKeyContext
    {
        builder.Services.AddScoped<IJwkStore, DatabaseJwkStore<TContext>>();

        return builder;
    }

    /// <summary>
    /// Sets the cache token
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static IJwksBuilder PersistCacheTokenToDatabaseStore<TContext>(this IJwksBuilder builder)
        where TContext : DbContext, IJwtCacheContext
    {
        builder.Services.AddScoped<IJwtStore, DatabaseJwtStore<TContext>>();

        return builder;
    }


}
