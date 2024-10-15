using ApplicationLayer.MappingConfigs;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer.DependencyInjection {
	public static class ServiceContainer {
		public static IServiceCollection AddApplicationService(this IServiceCollection services) {

			// Register automapper
			IMapper mapper = MappingExtension.RegisterMaps().CreateMapper();
			services.AddSingleton(mapper);
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			return services;
		}
	}
}
