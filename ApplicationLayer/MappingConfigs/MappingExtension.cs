using AutoMapper;

namespace ApplicationLayer.MappingConfigs {
	public class MappingExtension {
		public static MapperConfiguration RegisterMaps() {
			var mappingConfig = new MapperConfiguration(config => {

			});

			return mappingConfig;
		}
	}
}
