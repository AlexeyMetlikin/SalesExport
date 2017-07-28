using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Data;
using SalesReport.Models.Abstract;
using SalesReport.Models.ViewModel;
using SalesReport.Models.Entities;
using System.ComponentModel;

namespace SalesReport.Controllers
{
    public class ReportController : Controller
    {
        private ISalesContainer _salesContainer;    // Для обращения к БД
        private IEmailService _emailService;        // Для отправки Email
        private IExcelExport _excelExport;          // Для создания отчета в Excel

        public ReportController(ISalesContainer salesContainer, IEmailService emailService, IExcelExport excelExport)
        {
            _salesContainer = salesContainer;
            _emailService = emailService;
            _excelExport = excelExport;
        }
        
        public ViewResult Form()
        {
            ReportViewModel model = new ReportViewModel
            {
                EmailMessage = new ReportParams(),
                PeriodFrom = DateTime.Now,
                PeriodTo = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult SendReport(ReportParams EmailMessage, DateTime PeriodFrom, DateTime PeriodTo)
        {
            if (PeriodFrom.Date > PeriodTo.Date)    // Если дата окончания превышает дату начала периода - добавить ошибку уровня модели
            {
                ModelState.AddModelError("IncorrectPeriod", "Дата начала периода не может превышать дату окончания периода");
            }
            if (ModelState.IsValid)
            {
                // Выбираем из БД заказы в соответствии с указанным периодом
                var sales = _salesContainer.OrderDetails
                            .Where(s => s.Order.OrderDate >= PeriodFrom && s.Order.OrderDate <= PeriodTo)
                            .ToList();

                // Если есть заказы 
                if (sales.Count > 0)
                {
                    // Получаем полный путь до отчета xlsx
                    _excelExport.SetFullPath(Server.MapPath("~/Reports"), String.Format("Report_{0}.xlsx", DateTime.Now.ToShortDateString()));

                    // Формируем отчет
                    _excelExport.ExportExcel(FormReport(sales), "Отчет");

                    ViewBag.reportName = String.Format("Отчёт по продажам за период с {0} по {1}", PeriodFrom.ToShortDateString(), PeriodTo.ToShortDateString());

                    // Отправляем Email с отчетом
                    SendEmail(EmailMessage, new string[] { _excelExport.FullPath });

                    // Добавляем сообщение о результате формирования отчета
                    TempData["message"] = string.Format("Отчет был сфомирован и отправлен на электронную почту {0}", EmailMessage.To);
                }
                else
                {
                    TempData["message"] = string.Format("Не найдено данных для формирования отчета", ViewBag.reportName); // Если нет заказов в соответствии с указанным перодом
                }

                return PartialView("ReportTable", sales);   // Отображаем отчет на экране
            }
            // Если модель содержит ошибки - вовзращаем представление со списком ошибок
            return View("Form", new ReportViewModel { EmailMessage = EmailMessage, PeriodFrom = PeriodFrom, PeriodTo = PeriodTo });
        }

        // Метод формирования DataTable на основе выборки из БД для дальнейшего экспорта в Excel
        private DataTable FormReport(List<OrderDetail> sales)
        {
            // Формируем DataTable из выборки sales
            DataTable data = ListToDataTable(sales.Select(s => new { s.OrderID, s.Order.OrderDate, s.ProductID, s.Product.Name, s.Quantity, s.UnitPrice, s.Discount }).ToList());

            // Словарь для соответствия между наименованием колонки DataTable и ее заголовком
            Dictionary<string, string> headerColumns = new Dictionary<string, string>
            {
                { "OrderID",    "Номер заказа"},
                { "OrderDate",  "Дата заказа"},
                { "ProductID",  "Артикул товара"},
                { "Name",       "Наименование товара"},
                { "Quantity",   "Количество реализованных единиц товара"},
                { "UnitPrice",  "Цена реализации за единицу продукции"},
                { "Discount",   "Скидка"},
                { "TotalSum",   "Итоговая сумма"},
            };

            // Добавляем в DataTable вычисляемый столбец
            data.Columns.Add("TotalSum");
            for (int i = 1; i <= data.Rows.Count; i++)
            {
                data.Rows[i - 1]["TotalSum"] = "=E" + (i + 1) + "*(F" + (i + 1) + "*(1-G" + (i + 1) + "))";
            }

            // Изменяем заголовок для каждой колонки
            foreach (DataColumn column in data.Columns)
            {
                column.Caption = headerColumns[column.ColumnName];
            }

            return data;
        }

        // Метод формирования DataTable на основе выборки из БД
        private DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));  // получаем типы всех данных из типа T
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];

                // Добавляем колонки с нужным типом данных
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);   
            }

            object[] values = new object[properties.Count];
            foreach (var item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);   // Формируем массив данных для одной строки из списка
                }

                dataTable.Rows.Add(values); // Заполняем строки DataTable
            }
            return dataTable;
        }

        // Метод отправки Email
        public void SendEmail(ReportParams EmailMessage, string[] attachFiles)
        {
            EmailMessage.Subject = ViewBag.reportName;      // заголовок
            EmailMessage.From = "test.exercise@mail.ru";    // отправитель
            EmailMessage.Message = "Отчет во вложении";     // сообщение

            _emailService.Host = "smtp.mail.ru";            // адрес почтового сервера

            // Вызываем метод отправки Email. attachFiles - путь для вложений, fuse8Ex - пароль от почтового ящика test.exercise@mail.ru
            _emailService.Send(EmailMessage, attachFiles, "fuse8Ex");
        }
    }
}