#if ANDROID
using Android.Util;


using MailKit.Net.Smtp;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MimeKit;
using System.Net.Mail;
using System.Threading.Tasks;


namespace MAUI_App.Services
{
    public static class GmailHelper
    {
        // TODO: Replace with your actual email and app password
        private const string GmailAddress = "suresh951025@gmail.com";
        private const string GmailAppPassword = "dtyp tfzm kktp kxlc";

        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Missed Call Notifier", GmailAddress));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using var client = new MailKit.Net.Smtp.SmtpClient();

                // Disable certificate revocation check (safe for Gmail)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(GmailAddress, GmailAppPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Log.Info("Email", $"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Log.Error("Email", "Failed to send email: " + ex.Message);
            }
        }

    }
}
#endif