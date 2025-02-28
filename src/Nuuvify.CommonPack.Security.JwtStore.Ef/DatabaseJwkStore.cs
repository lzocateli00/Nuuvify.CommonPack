using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Nuuvify.CommonPack.Security.JwtCredentials.Model;
using Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;
using Nuuvify.CommonPack.Security.JwtCredentials;
using Nuuvify.CommonPack.Security.JwtCredentials.Jwks;

namespace Nuuvify.CommonPack.Security.JwtStore.Ef;

internal class DatabaseJwkStore<TContext> : IJwkStore
    where TContext : DbContext, ISecurityKeyContext
{
    private readonly TContext _context;
    private readonly IOptions<JwksOptions> _options;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DatabaseJwkStore<TContext>> _logger;

    public DatabaseJwkStore(TContext context,
        ILogger<DatabaseJwkStore<TContext>> logger,
        IOptions<JwksOptions> options,
        IMemoryCache memoryCache)
    {
        _context = context;
        _options = options;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public void Save(SecurityKeyWithPrivate securityParameteres)
    {
        _context.SecurityKeys.Add(securityParameteres);

        _logger.LogInformation($"Saving new SecurityKeyWithPrivate: {securityParameteres.Id}", typeof(TContext).Name);
        _context.SaveChanges();
        ClearCache();
    }

    public SecurityKeyWithPrivate GetCurrentKey()
    {
        if (!_memoryCache.TryGetValue(JwkContants.CurrentJwkCache, out SecurityKeyWithPrivate credentials))
        {
            credentials = _context.SecurityKeys.OrderByDescending(d => d.CreationDate).AsNoTracking().FirstOrDefault();
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(_options.Value.CacheTime);

            _memoryCache.Set(JwkContants.CurrentJwkCache, credentials, cacheEntryOptions);
        }
        // Put logger in a local such that `this` isn't captured.
        return _context.SecurityKeys.OrderByDescending(d => d.CreationDate).AsNoTracking().FirstOrDefault();
    }

    public IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5)
    {
        if (!_memoryCache.TryGetValue(JwkContants.JwksCache, out IReadOnlyCollection<SecurityKeyWithPrivate> keys))
        {
            keys = _context.SecurityKeys.OrderByDescending(d => d.CreationDate).Take(quantity).AsNoTracking().ToList().AsReadOnly();
            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(_options.Value.CacheTime);

            _memoryCache.Set(JwkContants.JwksCache, keys, cacheEntryOptions);
        }

        return _context.SecurityKeys.OrderByDescending(d => d.CreationDate).Take(quantity).AsNoTracking().ToList().AsReadOnly();
    }
    public bool NeedsUpdate()
    {
        var current = GetCurrentKey();
        if (current == null)
            return true;

        return current.CreationDate.AddDays(_options.Value.DaysUntilExpire) < DateTime.UtcNow.Date;
    }

    public void Clear()
    {
        foreach (var securityKeyWithPrivate in _context.SecurityKeys)
        {
            _context.SecurityKeys.Remove(securityKeyWithPrivate);
        }

        _context.SaveChanges();
        ClearCache();
    }


    public void Update(SecurityKeyWithPrivate securityKeyWithPrivate)
    {
        _context.SecurityKeys.Update(securityKeyWithPrivate);
        _context.SaveChanges();
        ClearCache();
    }

    private void ClearCache()
    {
        _memoryCache.Remove(JwkContants.JwksCache);
        _memoryCache.Remove(JwkContants.CurrentJwkCache);
    }
}
