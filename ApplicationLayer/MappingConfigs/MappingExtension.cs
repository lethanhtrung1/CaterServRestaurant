using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Responses.Category;
using AutoMapper;
using DomainLayer.Entites;

namespace ApplicationLayer.MappingConfigs {
	public class MappingExtension {
		public static MapperConfiguration RegisterMaps() {
			var mappingConfig = new MapperConfiguration(config => {
				#region Domain to ResponseDto

				config.CreateMap<Category, CategoryResponseDto>();

				#endregion


				#region RequestDto to Domain

				config.CreateMap<CreateCategoryRequest, Category>();
				config.CreateMap<UpdateCategoryRequest, Category>();

				#endregion
			});

			return mappingConfig;
		}
	}
}
