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
                    try
                    {
                        var jsonPath = Microsoft.Extensions.Configuration.UserSecrets.PathHelper.GetSecretsPathFromSecretsId(
                            "aspnet-PureMVC-03DBBA63-909B-4BCD-B387-84069AD9A5E3");
                        if (jsonPath == null)
                            return;
                        var rd = new System.IO.StreamReader(jsonPath);
                        var lines = rd.ReadToEndAsync();
                        var jsonConv = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailAppExt>(lines.Result);
                        if (jsonConv == null)
                            return;
                        smtp.Host = jsonConv.SMTPHost;
                        smtp.Port = jsonConv.SMTPPort;
                        smtp.EnableSsl = jsonConv.SSL;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = jsonConv.SMTPDefCredentials;
                        string userName = jsonConv.SMTPUserName;
                        string pass = jsonConv.SMTPPass;
                        smtp.Credentials = new System.Net.NetworkCredential(userName, pass);
                        using (var msg = new MailMessage(userName, email, subject, message))
                        {
                            msg.IsBodyHtml = true;
                            smtp.SendMailAsync(msg).Wait();
                        }
                    } catch (Exception)
                    { }
                }
            });
            task.Start();
            return task;
        }

        public partial class EmailAppExt
        {
            public bool SMTPDefCredentials { get; set; }
            public string SMTPHost { get; set; }
            public int SMTPPort { get; set; }
            public bool SSL { get; set; }
            public string SMTPUserName { get; set; }
            public string SMTPPass { get; set; }
        }
    }
}
