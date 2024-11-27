namespace ApplicationLayer.Options {
	public class CloudinaryOptions {
		public static string ConfigName => "Cloudinary";
		public string CloudName { get; set; }
		public string ApiKey { get; set; }
		public string ApiSecret { get; set; }
		public string Folder {  get; set; }
	}
}
