using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class UserProfileRepository : RepositoryBase<UserProfile>, IUserProfileRepository {
		public UserProfileRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
