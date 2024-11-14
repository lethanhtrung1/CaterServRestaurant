using ApplicationLayer.DTOs.Requests.Booking;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.DTOs.Requests.Meal;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.DTOs.Responses.Booking;
using ApplicationLayer.DTOs.Responses.Category;
using ApplicationLayer.DTOs.Responses.Coupon;
using ApplicationLayer.DTOs.Responses.Meal;
using ApplicationLayer.DTOs.Responses.Menu;
using ApplicationLayer.DTOs.Responses.Product;
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
				config.CreateMap<Booking, BookingResponse>();

				config.CreateMap<Product, ProductResponse>();
				config.CreateMap<Menu, ProductMenuDto>()
					.ForMember(
						dest => dest.Id,
						opt => opt.MapFrom(src => src.Id))
					.ForMember(
						dest => dest.MenuName,
						opt => opt.MapFrom(src => src.MenuName))
					.ForMember(
						dest => dest.Description,
						opt => opt.MapFrom(src => src.Description));

				config.CreateMap<Category, ProductCategoryDto>()
					.ForMember(
						dest => dest.Id,
						opt => opt.MapFrom(src => src.Id))
					.ForMember(
						dest => dest.Code,
						opt => opt.MapFrom(src => src.Code))
					.ForMember(
						dest => dest.Name,
						opt => opt.MapFrom(src => src.Name))
					.ForMember(
						dest => dest.Description,
						opt => opt.MapFrom(src => src.Description));
				config.CreateMap<ProductImage, ProductImageDto>();
				config.CreateMap<ProductImage, ProductImageResponse>();

				config.CreateMap<Meal, MealResponse>();
				config.CreateMap<MealProduct, MealProductResponse>();
				config.CreateMap<Product, MealProductDetailDto>();

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
				config.CreateMap<CreateBookingRequest, Booking>();
				config.CreateMap<UpdateBookingRequest, Booking>();

				config.CreateMap<CreateProductRequest, Product>();
				config.CreateMap<UpdateProductRequest, Product>();
				//config.CreateMap<CreateProductImageRequest, ProductImage>();

				#endregion
			});

			return mappingConfig;
		}
	}
}
