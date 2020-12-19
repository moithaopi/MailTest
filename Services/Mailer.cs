using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailTest.Contracts;
using MailTest.Entities;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using MimeKit;

namespace MailTest.Services
{
    public class Mailer : IMailer
    {
        private readonly SmtpSetting smtpSetting;
        private readonly IHostingEnvironment enviroment;

        public Mailer(IOptions<SmtpSetting> smtpSetting,IHostingEnvironment enviroment)
        {
            this.smtpSetting = smtpSetting.Value;
            this.enviroment = enviroment;
        }

        [Obsolete]
        public async Task SendEmailAsnyc(string email, string subject, string body)
        {
            try
            {
                var message=new MimeMessage();
                message.From.Add(new MailboxAddress(smtpSetting.SenderName,smtpSetting.SenderEmail));
                message.To.Add(new MailboxAddress(email));
                message.Subject=subject;
                message.Body=new TextPart("html"){
                    Text=body
                };
                using(var client=new SmtpClient()){
                    client.ServerCertificateValidationCallback=(s,c,h,e)=>true;
                    if(enviroment.IsDevelopment()){
                        client.Connect(smtpSetting.Server,smtpSetting.Port,false);
                    }else{
                        client.Connect(smtpSetting.Server);
                    }
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}