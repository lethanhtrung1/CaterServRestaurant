using ApplicationLayer.DTOs.Email;

namespace ApplicationLayer.Common.Consumer {
	public interface IEmailService {
		void SendEmail(Message message);
	}
}
