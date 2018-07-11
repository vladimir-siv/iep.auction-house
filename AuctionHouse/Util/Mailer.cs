using System.Text;
using System.Net;
using System.Net.Mail;

namespace AuctionHouse
{
	public static class Mailer
	{
		public static void SendMail(string fromEmail, string fromName, string toEmail, string toName, string subject, string body)
		{
			using (SmtpClient client = new SmtpClient(Settings.SMTPHost, Settings.SMTPPort))
			{
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential(Settings.SMTPUsername, Settings.SMTPPassword);

				using (MailMessage message = new MailMessage(new MailAddress(fromEmail, fromName, Encoding.UTF8), new MailAddress(toEmail, toName, Encoding.UTF8)))
				{
					message.Subject = subject;
					message.SubjectEncoding = Encoding.UTF8;

					message.Body = body;
					message.BodyEncoding = Encoding.UTF8;

					client.Send(message);
				}
			}
		}
	}
}