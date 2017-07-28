using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesReport.Models.Entities;
using SalesReport.Models.Abstract;
using Moq;
using System.Linq;
using System.Collections.Generic;
using SalesReport.Controllers;
using System.Web;
using System.Web.Mvc;
using System;
using SalesReport.Models.ViewModel;
using System.Data;
using System.IO;
using NSubstitute;

namespace SalesReport.Tests
{
    [TestClass]
    public class ReportControllerTest
    {
        [TestMethod]
        public void Can_Show_Form_View()
        {
            //Arrange
            // создаем Mock-контейнер и добавляем несколько экземпляров OrderDetail
            Mock<ISalesContainer> mock = new Mock<ISalesContainer>();
            mock.Setup(m => m.OrderDetails).Returns(new List<OrderDetail>
            {
                new OrderDetail
                {
                    ID = 1,
                    OrderID = 1,
                    Order = new Order { ID = 1, OrderDate = new System.DateTime(2001, 11, 1) },
                    ProductID = 1,
                    Product = new Product { ID = 1, Name = "Продукт 1" },
                    Quantity = 10,
                    Discount = 0.05f,
                    UnitPrice = 12
                },
                new OrderDetail
                {
                    ID = 2,
                    OrderID = 2,
                    Order = new Order { ID = 2, OrderDate = new System.DateTime(2001, 11, 1) },
                    ProductID = 2,
                    Product = new Product { ID = 2, Name = "Продукт 2" },
                    Quantity = 5,
                    Discount = 0.1f,
                    UnitPrice = 6
                }
            }.AsQueryable());

            IEmailService emailServise = new EmailServiceForTests();
            IExcelExport excelExport = new ExcelExportForTests();

            //Создаем экземпляр контроллера
            ReportController controller = new ReportController(mock.Object, emailServise, excelExport);

            //Act
            var reportViewModel = controller.Form().Model;

            //Assert
            // Сравниваем тип модели, полученной из контроллера, с ReportViewModel
            Assert.IsInstanceOfType(reportViewModel, typeof(ReportViewModel));

            // Сравниваем даты PeriodFrom и PeriodTo с текущей датой
            Assert.AreEqual((reportViewModel as ReportViewModel).PeriodFrom.Date, DateTime.Now.Date);
            Assert.AreEqual((reportViewModel as ReportViewModel).PeriodTo.Date, DateTime.Now.Date);
        }

        [TestMethod]
        public void Can_Send_Email_If_Model_Is_Valid()
        {
            // создаем Mock-контейнер и добавляем несколько экземпляров OrderDetail
            Mock<ISalesContainer> mock = new Mock<ISalesContainer>();
            mock.Setup(m => m.OrderDetails).Returns(new List<OrderDetail>
            {
                new OrderDetail
                {
                    ID = 1,
                    OrderID = 1,
                    Order = new Order { ID = 1, OrderDate = new System.DateTime(2001, 12, 1) },
                    ProductID = 1,
                    Product = new Product { ID = 1, Name = "Продукт 1" },
                    Quantity = 10,
                    Discount = 0.05f,
                    UnitPrice = 12
                },
                new OrderDetail
                {
                    ID = 2,
                    OrderID = 2,
                    Order = new Order { ID = 2, OrderDate = new System.DateTime(2001, 11, 1) },
                    ProductID = 2,
                    Product = new Product { ID = 2, Name = "Продукт 2" },
                    Quantity = 5,
                    Discount = 0.1f,
                    UnitPrice = 6
                }
            }.AsQueryable());

            //Создаем HttpContext для контроллера
            var mockHttpContext = Substitute.For<HttpContextBase>();

            IEmailService emailServise = new EmailServiceForTests();
            IExcelExport excelExport = new ExcelExportForTests();

            //Создаем экземпляр контроллера и присваиваем HttpContext
            ReportController controller = new ReportController(mock.Object, emailServise, excelExport);
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = mockHttpContext
            };

            // Act - вызываем метод отправки Email
            ActionResult result = controller.SendReport(new ReportParams { To = "test@mail.ru" }, new DateTime(2001, 11, 1), new DateTime(2001, 12, 1));

            // Assert
            // Проверяем, что результатом метода является частичное представление
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            // Проверяем наименование частичного представления
            Assert.AreEqual(((PartialViewResult)(result)).ViewName, "ReportTable");

            
        }

        [TestMethod]
        public void Cannot_Send_Mail_If_Model_Is_Invalid()
        {
            // создаем Mock-контейнер и добавляем несколько экземпляров OrderDetail
            Mock<ISalesContainer> mock = new Mock<ISalesContainer>();
            mock.Setup(m => m.OrderDetails).Returns(new List<OrderDetail>
            {
                new OrderDetail
                {
                    ID = 1,
                    OrderID = 1,
                    Order = new Order { ID = 1, OrderDate = new System.DateTime(2001, 12, 1) },
                    ProductID = 1,
                    Product = new Product { ID = 1, Name = "Продукт 1" },
                    Quantity = 10,
                    Discount = 0.05f,
                    UnitPrice = 12
                },
                new OrderDetail
                {
                    ID = 2,
                    OrderID = 2,
                    Order = new Order { ID = 2, OrderDate = new System.DateTime(2001, 11, 1) },
                    ProductID = 2,
                    Product = new Product { ID = 2, Name = "Продукт 2" },
                    Quantity = 5,
                    Discount = 0.1f,
                    UnitPrice = 6
                }
            }.AsQueryable());

            //Создаем HttpContext для контроллера
            var mockHttpContext = Substitute.For<HttpContextBase>();

            IEmailService emailServise = new EmailServiceForTests();
            IExcelExport excelExport = new ExcelExportForTests();

            //Создаем экземпляр контроллера и присваиваем HttpContext
            ReportController controller = new ReportController(mock.Object, emailServise, excelExport);
            controller.ControllerContext = new ControllerContext
            {
                Controller = controller,
                HttpContext = mockHttpContext
            };

            /// Act - вызываем метод отправки Email 
            ActionResult result = controller.SendReport(new ReportParams { To = "test@mail.ru" }, new DateTime(2001, 12, 1), new DateTime(2001, 11, 1));

            //Assert
            // Проверяем, что результатом метода является представление (ViewResult)
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Проверяем наименование представления
            Assert.AreEqual(((ViewResult)(result)).ViewName, "Form");
        }

        // Класс-заглушка, реализует абстрактный класс IExcelExport
        private class ExcelExportForTests : IExcelExport
        {
            // Переопределение метода формирования отчета
            public override void ExportExcel(DataTable dataTable, string heading = "", int colHeaderRows = 1)
            {
                return;
            }
        }

        // Класс-заглушка, реализует интерфейс IEmailService
        private class EmailServiceForTests : IEmailService
        {
            public string Host { get; set; }

            public void Send(ReportParams EmailMessage, string[] attachFiles, string pass = null)
            {
                return;
            }
        }
    }
}
