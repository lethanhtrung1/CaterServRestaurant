﻿using DomainLayer.Common;
using DomainLayer.Entites;

namespace DomainLayer.Repositories {
	public interface IBookingRepository : IRepositoryBase<Booking> {
		Task<Booking?> GetLatestBookingByTableIdAsync(Guid? tableId);
	}
}
