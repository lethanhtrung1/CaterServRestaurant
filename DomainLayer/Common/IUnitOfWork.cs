namespace DomainLayer.Common {
	public interface IUnitOfWork : IDisposable {
		Task SaveChangeAsync();
	}
}
