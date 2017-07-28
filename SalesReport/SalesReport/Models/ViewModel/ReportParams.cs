using System.ComponentModel.DataAnnotations;

namespace SalesReport.Models.ViewModel
{
    public class ReportParams
    {
        // Заголовок сообщения
        public string Subject { get; set; }
        
        // Отправитель
        public string From { get; set; }

        // Получатель
        [Required(ErrorMessage = "Необходимо ввести адрес электронной почты")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Некорректно введен адрес электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        public string To { get; set; }

        // Текст сообщения
        public string Message { get; set; }
    }
}