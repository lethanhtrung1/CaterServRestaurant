using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace InfrastructrureLayer.Common {
	public class ConnectionHelper {
		private static Lazy<ConnectionMultiplexer> lazyConnection;

		public ConnectionHelper(IConfiguration configuration) {
			string redisUrl = configuration.GetValue<string>("RedisURL") ?? "127.0.0.1:6379";
			if (string.IsNullOrEmpty(redisUrl)) {
				throw new ArgumentException("RedisURL configuration is missing or invalid");
			}
			lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
				return ConnectionMultiplexer.Connect(redisUrl);
			});
		}

		public ConnectionMultiplexer Connection => lazyConnection.Value;
	}
}
