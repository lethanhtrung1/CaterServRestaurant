﻿using DomainLayer.Common;
using DomainLayer.Entites;

namespace DomainLayer.Repositories {
	public interface IApplicationUserRepository : IRepository<ApplicationUser> {
		Task<int> GetTotalUsers();
	}
}
