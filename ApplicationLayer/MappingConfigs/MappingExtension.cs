using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Responses.Category;
using ApplicationLayer.DTOs.Responses.Menu;
using AutoMapper;
using DomainLayer.Entites;

namespace ApplicationLayer.MappingConfigs {
	public class MappingExtension {
		public static MapperConfiguration RegisterMaps() {
			var mappingConfig = new MapperConfiguration(config => {
				#region Domain to ResponseDto

				config.CreateMap<Category, CategoryResponseDto>();
				config.CreateMap<Menu, MenuResponse>();

				#endregion


				#region RequestDto to Domain

				config.CreateMap<CreateCategoryRequest, Category>();
				config.CreateMap<UpdateCategoryRequest, Category>();

				config.CreateMap<CreateMenuRequest, Menu>();
				config.CreateMap<UpdateMenuRequest, Menu>();

				#endregion
			});

			return mappingConfig;
		}
	}
}
