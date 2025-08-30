    using Eticaret.Core.Entities;
    using System.Net;
    using System.Net.Mail;



    namespace Eticaret.WebUI.Utills
    {
        public class MailHelper
        {
            public static async Task SendMailAsync(Contact contact)
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("emrekaya241907@gmail.com", "iplragwdptdppxpj");
                smtpClient.EnableSsl = true;
                MailMessage message = new MailMessage();
                message.From = new MailAddress("emrekaya241907@gmail.com");
                message.To.Add("emrekaya241907@gmail.com");
                message.Subject = "Yeni bir mesajınız var.";
                message.Body = $"isim: {contact.Name} <br> Soyisim: {contact.Surname} <br> Email: {contact.Email} <br> Telefon: {contact.Phone} <br> Mesaj: {contact.Message}";
                message.IsBodyHtml = true;
                await smtpClient.SendMailAsync(message);
                smtpClient.Dispose();
             
            }
        }
    }
