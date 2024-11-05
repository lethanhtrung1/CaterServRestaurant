using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.DTOs.Responses.Category;
using ApplicationLayer.DTOs.Responses.Coupon;
using ApplicationLayer.DTOs.Responses.Menu;
using ApplicationLayer.DTOs.Responses.Table;
using AutoMapper;
using DomainLayer.Entites;

namespace ApplicationLayer.MappingConfigs {
	public class MappingExtension {
		public static MapperConfiguration RegisterMaps() {
			var mappingConfig = new MapperConfiguration(config => {
				#region Domain to ResponseDto

				config.CreateMap<Category, CategoryResponseDto>();
				config.CreateMap<Menu, MenuResponse>();
				config.CreateMap<Table, TableResponse>();
				config.CreateMap<Coupon, CouponResponse>();

				#endregion

				#region RequestDto to Domain

				config.CreateMap<CreateCategoryRequest, Category>();
				config.CreateMap<UpdateCategoryRequest, Category>();
				config.CreateMap<CreateMenuRequest, Menu>();
				config.CreateMap<UpdateMenuRequest, Menu>();
				config.CreateMap<CreateTableRequest, Table>();
				config.CreateMap<UpdateTableRequest, Table>();
				config.CreateMap<CreateCouponRequest, Coupon>();
				config.CreateMap<UpdateCouponRequest, Coupon>();

				#endregion
			});

			return mappingConfig;
		}
	}
}
