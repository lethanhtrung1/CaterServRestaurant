using System.Net;

namespace ApplicationLayer.Utilities {
	public static class ObjectExtensions {
		public static string ToQueryString(this object obj) {
			var properties = from p in obj.GetType().GetProperties()
							 where p.GetValue(obj, null) != null
							 select p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null)?.ToString());
			string queryString = string.Join("&", properties.ToArray());
			return queryString;
		}
	}
}
