using ApplicationLayer.DTOs.Requests.Booking;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Requests.Merchant;
using ApplicationLayer.DTOs.Requests.PaymetDestination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.DTOs.Responses.Booking;
using ApplicationLayer.DTOs.Responses.Category;
using ApplicationLayer.DTOs.Responses.Coupon;
using ApplicationLayer.DTOs.Responses.Meal;
using ApplicationLayer.DTOs.Responses.Menu;
using ApplicationLayer.DTOs.Responses.Merchant;
using ApplicationLayer.DTOs.Responses.Order;
using ApplicationLayer.DTOs.Responses.PaymentDestination;
using ApplicationLayer.DTOs.Responses.Product;
using ApplicationLayer.DTOs.Responses.Review;
using ApplicationLayer.DTOs.Responses.Table;
using ApplicationLayer.DTOs.Responses.User;
using ApplicationLayer.DTOs.Responses.UserProfile;
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
				config.CreateMap<Menu, ProductMenuDto>();
				config.CreateMap<Category, ProductCategoryDto>();
				config.CreateMap<ProductImage, ProductImageDto>();
				config.CreateMap<ProductImage, ProductImageResponse>();
				config.CreateMap<Meal, MealResponse>();
				config.CreateMap<MealProduct, MealProductResponse>();
				config.CreateMap<Product, MealProductDetailDto>();
				config.CreateMap<Order, OrderResponse>();
				config.CreateMap<OrderDetail, OrderDetailResponse>();
				config.CreateMap<Merchant, MerchantResponse>();
				config.CreateMap<PaymentDestination, PaymentDestinationResponse>();

				config.CreateMap<ApplicationUser, UserResponse>()
					.ForMember(
						dest => dest.IsBanned,
						opt => opt.MapFrom(
							src => src.LockoutEnd.HasValue && src.LockoutEnd.Value > DateTime.Now
						)
					);

				config.CreateMap<UserProfile, UserProfileResponse>();
				config.CreateMap<UserProfile, StaffProfileResponse>();

				config.CreateMap<Review, ReviewResponse>();

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
				config.CreateMap<CreateMerchantRequest, Merchant>();
				config.CreateMap<UpdateMerchantRequest, Merchant>();
				config.CreateMap<CreatePaymentDestinationRequest, PaymentDestination>();
				config.CreateMap<UpdatePaymentDestinationRequest, PaymentDestination>();
				#endregion
			});

			return mappingConfig;
		}
	}
}
