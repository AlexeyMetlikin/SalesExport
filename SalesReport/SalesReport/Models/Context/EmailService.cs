using SalesReport.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalesReport.Models.ViewModel;
using System.Net.Mail;
using System.Net;

namespace SalesReport.Models.Context
{
    public class EmailService : IEmailService
    {
        public string Host { get; set; }    // Адрес почтового сервера

        public void Send(ReportParams EmailMessage, string[] attachFiles, string pass = null)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(EmailMessage.From);  // отправитель            
                mailMessage.To.Add(new MailAddress(EmailMessage.To));   // получатель            
                mailMessage.Subject = EmailMessage.Subject;             // заголовок            
                mailMessage.IsBodyHtml = true;                          // тип сообщений            
                mailMessage.Body = EmailMessage.Message;                // сообщение

                // Если нужны вложения
                if (attachFiles != null)
                {
                    // Прикрепляем файлы к сообщению
                    foreach (var attach in attachFiles)
                    {
                        if (!string.IsNullOrEmpty(attach))
                        {
                            mailMessage.Attachments.Add(new Attachment(attach));
                        }
                    }
                }

                // Сервис для отправки сообщения
                SmtpClient client = new SmtpClient(Host, 25);
                client.Credentials = new NetworkCredential(EmailMessage.From.Split('@')[0], pass);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                // отправка сообщения
                client.Send(mailMessage);

                mailMessage.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception("Mail.Send: " + e.Message);
            }
        }
    }
}