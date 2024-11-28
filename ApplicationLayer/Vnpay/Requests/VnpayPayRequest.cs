using ApplicationLayer.Utilities;
using ApplicationLayer.Vnpay.Lib;
using System.Net;
using System.Text;

namespace ApplicationLayer.Vnpay.Requests {
	public class VnpayPayRequest {
		public VnpayPayRequest() { }

		public VnpayPayRequest(string version, string tmnCode, DateTime createDate, string ipAddress,
			decimal amount, string currCode, string orderType, string orderInfo,
			string returnUrl, string txnRef) {

			vnp_Version = version;
			vnp_TmnCode = tmnCode;
			vnp_CreateDate = createDate.ToString("yyyyMMddHHmmss");
			vnp_IpAddr = ipAddress;
			vnp_Amount = (int)amount * 100;
			vnp_CurrCode = currCode;
			vnp_OrderType = orderType;
			vnp_OrderInfo = orderInfo;
			vnp_ReturnUrl = returnUrl;
			vnp_TxnRef = txnRef;
			vnp_Locale = "vn";
			vnp_Command = "pay";
		}

		public SortedList<string, string> requestData = new SortedList<string, string>(new VnpayCompare());

		public void MakeRequestData() {
			if (vnp_Amount != null)
				requestData.Add("vnp_Amount", vnp_Amount.ToString() ?? string.Empty);
			if (vnp_Command != null)
				requestData.Add("vnp_Command", vnp_Command);
			if (vnp_CreateDate != null)
				requestData.Add("vnp_CreateDate", vnp_CreateDate);
			if (vnp_CurrCode != null)
				requestData.Add("vnp_CurrCode", vnp_CurrCode);
			if (vnp_BankCode != null)
				requestData.Add("vnp_BankCode", vnp_BankCode);
			if (vnp_IpAddr != null)
				requestData.Add("vnp_IpAddr", vnp_IpAddr);
			if (vnp_Locale != null)
				requestData.Add("vnp_Locale", vnp_Locale);
			if (vnp_OrderInfo != null)
				requestData.Add("vnp_OrderInfo", vnp_OrderInfo);
			if (vnp_OrderType != null)
				requestData.Add("vnp_OrderType", vnp_OrderType);
			if (vnp_ReturnUrl != null)
				requestData.Add("vnp_ReturnUrl", vnp_ReturnUrl);
			if (vnp_TmnCode != null)
				requestData.Add("vnp_TmnCode", vnp_TmnCode);
			if (vnp_ExpireDate != null)
				requestData.Add("vnp_ExpireDate", vnp_ExpireDate);
			if (vnp_TxnRef != null)
				requestData.Add("vnp_TxnRef", vnp_TxnRef);
			if (vnp_Version != null)
				requestData.Add("vnp_Version", vnp_Version);
		}

		public string GetLink(string baseUrl, string secretKey) {
			MakeRequestData();
			StringBuilder data = new StringBuilder();

			foreach (KeyValuePair<string, string> keyValue in requestData) {
				if (!string.IsNullOrEmpty(keyValue.Value)) {
					data.Append(WebUtility.UrlEncode(keyValue.Key) + "=" + WebUtility.UrlEncode(keyValue.Value) + "&");
				}
			}

			string result = baseUrl + "?" + data.ToString();
			var secureHash = HashHelper.HmacSHA521(secretKey, data.ToString().Remove(data.Length - 1, 1));

			return result += "vnp_SecureHash=" + secureHash;
		}


		public string? vnp_Version { get; set; } // Phiên bản api mà merchant kết nối. Phiên bản hiện tại là : 2.1.0
		public string? vnp_TmnCode { get; set; } // Mã website của merchant trên hệ thống của VNPAY. Ví dụ: 2QXUI4J4
		public string? vnp_CreateDate { get; set; } // Là thời gian phát sinh giao dịch định dạng yyyyMMddHHmmss (Time zone GMT+7) Ví dụ: 20220101103111
		public string? vnp_ExpireDate { get; set; } // Thời gian hết hạn thanh toán GMT+7, định dạng: yyyyMMddHHmmss
		public string? vnp_IpAddr { get; set; } // Địa chỉ IP của khách hàng thực hiện giao dịch. Ví dụ: 13.160.92.202
		public string? vnp_BankCode { get; set; } // Mã phương thức thanh toán, mã loại ngân hàng hoặc ví điện tử thanh toán.
		public decimal? vnp_Amount { get; set; }  // Số tiền thanh toán
		public string? vnp_CurrCode { get; set; } // Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
		public string? vnp_OrderType { get; set; } // Mã danh mục hàng hóa (other)
		public string? vnp_OrderInfo { get; set; } // Thông tin mô tả nội dung thanh toán vd: Nap tien cho thue bao 0123456789. So tien 100,000 VND
		public string? vnp_ReturnUrl { get; set; } // URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán. Ví dụ: https://domain.vn/VnPayReturn
		public string? vnp_TxnRef { get; set; } // Mã tham chiếu của giao dịch tại hệ thống của merchant
		public string? vnp_Locale { get; set; } // Ngôn ngữ giao diện hiển thị. Hiện tại hỗ trợ Tiếng Việt (vn), Tiếng Anh (en)
		public string? vnp_Command { get; set; } // Mã API sử dụng, mã cho giao dịch thanh toán là: pay
		public string? vnp_SecureHash { get; set; } // Mã kiểm tra (checksum) để đảm bảo dữ liệu của giao dịch không bị thay đổi trong quá trình chuyển từ merchant sang VNPAY
	}
}
