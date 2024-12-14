using DomainLayer.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace InfrastructrureLayer.Common {
	public class CacheService : ICacheService {
		private readonly IDatabase _cacheDb;

		public CacheService(ConnectionHelper connectionHelper) {
			_cacheDb = connectionHelper.Connection.GetDatabase();
		}

		public async Task<T> GetData<T>(string key) {
			var value = await _cacheDb.StringGetAsync(key);
			return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value);
		}

		public async Task<IEnumerable<string>> GetKeys(string pattern) {
			try {
				var endpoint = _cacheDb.Multiplexer.GetEndPoints().First();
				var server = _cacheDb.Multiplexer.GetServer(endpoint);

				return await Task.Run(() =>
					server.Keys(pattern: pattern).Select(k => k.ToString()).ToList()
				);
			} catch (Exception) {
				return Enumerable.Empty<string>();
			}
		}

		public async Task<object> RemoveData(string key) {
			bool isExist = await _cacheDb.KeyExistsAsync(key);
			return isExist ? _cacheDb.KeyDelete(key) : false;
		}

		public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime) {
			TimeSpan expireTime = expirationTime.DateTime.Subtract(DateTime.UtcNow);
			var isSet = await _cacheDb.StringSetAsync(key, JsonConvert.SerializeObject(value), expireTime);
			return isSet;
		}

		public async Task RemoveMultipleKeysAsync(string pattern) {
			var keys = await GetKeys(pattern);
			var tasks = keys.Select(key => RemoveData(key));
			await Task.WhenAll(tasks);
		}
	}
}
