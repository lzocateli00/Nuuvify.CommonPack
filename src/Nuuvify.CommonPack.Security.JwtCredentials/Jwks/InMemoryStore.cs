
using Nuuvify.CommonPack.Security.JwtCredentials.Interfaces;
using Nuuvify.CommonPack.Security.JwtCredentials.Model;
using Microsoft.Extensions.Options;

namespace Nuuvify.CommonPack.Security.JwtCredentials.Jwks;

public class InMemoryStore : IJwkStore
{
    private readonly IOptions<JwksOptions> _options;
    static readonly object lockObject = new object();
    private readonly List<SecurityKeyWithPrivate> _store;
    private SecurityKeyWithPrivate _current;

    public InMemoryStore(IOptions<JwksOptions> options)
    {
        _options = options;
        _store = new List<SecurityKeyWithPrivate>();
    }

    public void Save(SecurityKeyWithPrivate securityParameteres)
    {
        lock (lockObject)
            _store.Add(securityParameteres);

        _current = securityParameteres;
    }

    public bool NeedsUpdate()
    {
        if (_current == null)
            return true;

        return _current.CreationDate.AddDays(_options.Value.DaysUntilExpire) < DateTimeOffset.Now.Date;
    }

    public void Update(SecurityKeyWithPrivate securityKeyWithPrivate)
    {
        var oldOne = _store.Find(f => f.Id == securityKeyWithPrivate.Id);
        if (oldOne != null)
        {
            var index = _store.FindIndex(f => f.Id == securityKeyWithPrivate.Id);
            Monitor.Enter(lockObject);
            _store.RemoveAt(index);
            _store.Insert(index, securityKeyWithPrivate);
            Monitor.Exit(lockObject);
        }
    }


    public SecurityKeyWithPrivate GetCurrentKey()
    {
        return _current;
    }

    public IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5)
    {
        return
            _store
                .OrderByDescending(s => s.CreationDate)
                .Take(quantity).ToList().AsReadOnly();
    }

    public void Clear()
    {
        _store.Clear();
    }
}
