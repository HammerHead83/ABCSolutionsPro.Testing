using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PureMVC.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            Task task = new Task(() => {
                using (var smtp = new SmtpClient())
                {
                    var jsonPath = Microsoft.Extensions.Configuration.UserSecrets.PathHelper.GetSecretsPathFromSecretsId("aspnet-PureMVC-03DBBA63-909B-4BCD-B387-84069AD9A5E3");
                    var rd = new System.IO.StreamReader(jsonPath);
                    var lines = rd.ReadToEndAsync();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    var jsonConv = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleAppExt>(lines.Result);
                    string userName = jsonConv.SMTPUserName;
                    string pass = jsonConv.SMTPPass;
                    smtp.Credentials = new System.Net.NetworkCredential(userName, pass);
                    using (var msg = new MailMessage(userName, email, subject, message))
                    {
                        msg.IsBodyHtml = true;
                        smtp.SendMailAsync(msg).Wait();
                    }
                }
            });
            task.Start();
            return task;
        }

        public partial class GoogleAppExt
        {
            public string SMTPUserName { get; set; }
            public string SMTPPass { get; set; }
        }
    }
}
