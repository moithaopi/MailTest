using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailTest.Contracts;
using MailTest.Entities;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailTest.Services
{
    public class Mailer : IMailer
    {
        private readonly SmtpSetting smtpSetting;
        public Mailer(IOptions<SmtpSetting> smtpSetting)
        {
            this.smtpSetting = smtpSetting.Value;

        }
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
                    await client.ConnectAsync(smtpSetting.Server,smtpSetting.Port,true);
                    await client.AuthenticateAsync(smtpSetting.Username,smtpSetting.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}