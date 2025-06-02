using MimeKit;
using System.Net.Mail;

namespace CI.Utility
{
    public class EmailUtility
    {        
        public bool SendEmailPasswordReset(string userEmail, string link)
        {
            int port = 587;
            string host = "smtp.office365.com";
            string username = "smtp.out@mail.com";
            string password = "password";
            string mailFrom = "noreply@mail.com";
            string mailTo = userEmail;
            string mailTitle = "Testtitle";
            string mailMessage = "Testmessage";

            var message = new MimeMessage();
            //message.From.Add(new MailboxAddress(mailFrom));
            //message.To.Add(new MailboxAddress(mailTo));
            //message.Subject = mailTitle;
            //message.Body = new TextPart("plain") { Text = mailMessage };

            //using (var client = new SmtpClient())
            //{
            //    client.Connect(host, port, SecureSocketOptions.StartTls);
            //    client.Authenticate(username, password);

            //    client.Send(message);
            //    client.Disconnect(true);
            //    return true;
            //}

            return true;
        }
    }
}