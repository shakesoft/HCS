using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;

namespace HC.Blazor.Components;

/// <summary>
/// Distributed session store for cookie authentication using Redis.
/// This reduces cookie size by storing authentication ticket in Redis instead of cookie.
/// Cookie will only contain session ID (~100 bytes) instead of full ticket (~36KB).
/// </summary>
public class DistributedCookieAuthenticationSessionStore : ITicketStore
{
    private readonly IDatabase _database;
    private readonly string _keyPrefix;
    private const int DefaultExpirationMinutes = 480; // 8 hours

    public DistributedCookieAuthenticationSessionStore(IDatabase database, string keyPrefix)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _keyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"{_keyPrefix}{Guid.NewGuid():N}";
        var serializedTicket = SerializeTicket(ticket);
        
        await _database.StringSetAsync(
            key,
            serializedTicket,
            TimeSpan.FromMinutes(DefaultExpirationMinutes),
            When.Always);
        
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var serializedTicket = SerializeTicket(ticket);
        
        await _database.StringSetAsync(
            key,
            serializedTicket,
            TimeSpan.FromMinutes(DefaultExpirationMinutes),
            When.Always);
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            return null;
        }
        
        return DeserializeTicket(value);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    private byte[] SerializeTicket(AuthenticationTicket ticket)
    {
        // Use ASP.NET Core's built-in ticket serializer
        return TicketSerializer.Default.Serialize(ticket);
    }

    private AuthenticationTicket? DeserializeTicket(RedisValue value)
    {
        if (!value.HasValue || value.IsNullOrEmpty)
        {
            return null;
        }

        // RedisValue can be implicitly converted to byte[]
        var bytes = (byte[])value;
        return TicketSerializer.Default.Deserialize(bytes);
    }
}
