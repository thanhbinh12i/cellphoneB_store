
using System.Net.Mail;
using System.Net;

namespace CellPhoneB_Store.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("binhnptse181532@fpt.edu.vn", "tojw jovv vwrc tppo")
            };

            return client.SendMailAsync(
                new MailMessage(from: "demologin979@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
