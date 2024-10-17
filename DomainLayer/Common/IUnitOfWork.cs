using DomainLayer.Repositories;

namespace DomainLayer.Common {
	public interface IUnitOfWork : IDisposable {
		Task SaveChangeAsync();
		IRefreshTokenRepository RefreshToken { get; }
	}
}
