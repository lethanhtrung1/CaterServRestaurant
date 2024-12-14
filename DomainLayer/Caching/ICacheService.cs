namespace DomainLayer.Caching {
	public interface ICacheService {
		Task<T> GetData<T>(string key);
		Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime);
		Task<object> RemoveData(string key);
		Task<IEnumerable<string>> GetKeys(string pattern);
		Task RemoveMultipleKeysAsync(string pattern);
	}
}
