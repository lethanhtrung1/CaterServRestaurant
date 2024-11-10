﻿using ApplicationLayer.Interfaces;
using ApplicationLayer.MappingConfigs;
using ApplicationLayer.Services;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer.DependencyInjection {
	public static class ServiceContainer {
		public static IServiceCollection AddApplicationService(this IServiceCollection services) {

			// Register automapper
			IMapper mapper = MappingExtension.RegisterMaps().CreateMapper();
			services.AddSingleton(mapper);
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			// DI
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IMenuService, MenuService>();
			services.AddScoped<ITableService, TableService>();
			services.AddScoped<ICouponService, CouponService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IProductImageService, ProductImageService>();
			services.AddScoped<IMealService, MealService>();

			return services;
		}
	}
}
