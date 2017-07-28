using System;
using System.ComponentModel.DataAnnotations;


namespace SalesReport.Models.ViewModel
{
    public class ReportViewModel
    {
        public ReportParams EmailMessage { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить дату начала периода")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата начала периода")]
        public DateTime PeriodFrom { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить дату окончания периода")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата окончания периода")]
        public DateTime PeriodTo { get; set; }
    }
}