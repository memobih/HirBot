using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HirBot.Redies
{
    public  class RedisService
    {

        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _database;
        
        public RedisService(string host, string password, int port, string username)
        {
            var options = ConfigurationOptions.Parse($"{host}:{port}");
            options.Password = password;
            options.User = username;
            _redisConnection = ConnectionMultiplexer.Connect(options);
            _database = _redisConnection.GetDatabase(0);
        }


        public async Task<List<string>> StoreJwtTokenAsync(string token, TimeSpan expiry)
        {
            var server =_redisConnection.GetServer("redis-14070.c339.eu-west-3-1.ec2.redns.redis-cloud.com:14070");
            var keys = server.Keys(pattern: "*");
            List<string> strings = new List<string>();
            foreach (var key in keys)
            {
                strings.Add(key);
            }
            _database.StringSet(token, "blacklist" , expiry);
            return strings;
        }
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _database.KeyExistsAsync(token);
        }
    } 
}
