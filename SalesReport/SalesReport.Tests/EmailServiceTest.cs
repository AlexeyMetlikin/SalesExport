using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesReport.Models.Abstract;
using Moq;
using SalesReport.Controllers;
using SalesReport.Models.ViewModel;

namespace SalesReport.Tests
{
    [TestClass]
    public class EmailServiceTest
    {
        [TestMethod]
        public void Can_Send_Email_With_Valid_Params()
        {
            //Arrange
            // создаем Mock-контейнер для IEmailService
            Mock<IEmailService> mockService = new Mock<IEmailService>();

            // Инициализируем контроллер
            ReportController controller = new ReportController(null, mockService.Object, null);

            //Act
            // Вызываем метод контроллера для отправки Email
            controller.SendEmail(new ReportParams { To = "test@mail.ru" }, new string[] { "attach.txt" });

            //Assert
            // Проверяем, что метод отправки Email был вызван один раз для объекта из mock-контейнера 
            mockService.Verify(m => m.Send(It.IsAny<ReportParams>(), It.IsAny<string[]>(), It.IsAny<string>()), Times.Once);
        }
    }
}
