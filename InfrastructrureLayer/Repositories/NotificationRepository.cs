using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository {
		public NotificationRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
