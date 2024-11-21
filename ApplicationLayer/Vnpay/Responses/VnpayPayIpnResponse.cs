namespace ApplicationLayer.Vnpay.Responses
{
    public class VnpayPayIpnResponse
    {
        public VnpayPayIpnResponse() { }

        public VnpayPayIpnResponse(string responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }

        public void Set(string responseCode, string message)
        {
            ResponseCode = ResponseCode;
            Message = message;
        }

        public string ResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
