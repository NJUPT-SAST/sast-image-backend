﻿using Account.Application.Services;
using StackExchange.Redis;

namespace Account.Infrastructure.Services
{
    internal sealed class RedisAuthCache(IConnectionMultiplexer connection) : IAuthCache
    {
        private readonly IDatabase _database = connection.GetDatabase();

        public Task StoreCodeAsync(string key, string code, TimeSpan expiry)
        {
            return _database
                .HashSetAsync(CacheKeys.RegistrationCodes, key, code)
                .ContinueWith(async t =>
                {
                    await Task.Delay(expiry);
                    _ = _database.HashDeleteAsync(CacheKeys.RegistrationCodes, key);
                });
        }

        public Task StoreCodeAsync(string key, string code)
        {
            return StoreCodeAsync(key, code, TimeSpan.FromMinutes(5));
        }

        public async Task<bool> VerifyCodeAsync(string key, string code)
        {
            var cacheCode = await _database.HashGetAsync(CacheKeys.RegistrationCodes, key);
            if (cacheCode.IsNull)
                return false;
            else
                return cacheCode == code;
        }
    }
}
