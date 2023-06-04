using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace Muzique_Api.Helpers
{
    public class SendEmail
    {
        public async Task SendEmailAsync(string toEmail,string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("dophucdai3051@gmail.com");
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Reset Password";
            var builder = new BodyBuilder();
            
            builder.HtmlBody = message;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("dophucdai3051@gmail.com", "abzdutxafixdpjly");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
