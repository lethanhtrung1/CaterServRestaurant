using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Email;
using ApplicationLayer.Options;
using System.Net;
using System.Net.Mail;

namespace InfrastructrureLayer.Services {
	public class EmailService : IEmailService {
		private readonly EmailOptions _emailOptions;

		public EmailService(EmailOptions emailOptions) {
			_emailOptions = emailOptions;
		}

		public void SendEmail(Message message) {
			MailMessage mailMessage = new MailMessage() {
				From = new MailAddress(_emailOptions.From),
				Subject = message.Subject,
				Body = message.Content
			};

			mailMessage.To.Add(message.To);

			using var smtpClient = new SmtpClient();
			smtpClient.Host = _emailOptions.Host;
			smtpClient.Port = _emailOptions.Port;
			smtpClient.Credentials = new NetworkCredential(_emailOptions.Username, _emailOptions.Password);
			smtpClient.EnableSsl = true;
			smtpClient.Send(mailMessage);
		}
	}
}
