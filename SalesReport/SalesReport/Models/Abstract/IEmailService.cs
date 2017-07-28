using SalesReport.Models.ViewModel;

namespace SalesReport.Models.Abstract
{
    public interface IEmailService
    {
        string Host { get; set; }   // Наименование почтового сервера

        // Метод отправки Email: 
        // EmailMessage - параметры сообщения (заголовок, сообщение, кому, от кого)
        // attachFiles - список путей до вложений на сервере
        // pass - пароль для авторизации в почте
        void Send(ReportParams EmailMessage, string[] attachFiles, string pass = null); 
    }
}