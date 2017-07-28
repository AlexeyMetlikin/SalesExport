using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesReport.Models.Context;
using SalesReport.Models.Abstract;
using System.Data;
using System.IO;

namespace SalesReport.Tests
{
    [TestClass]
    public class ExcelExportTest
    {
        [TestMethod]
        public void Can_Export_To_Excel()
        {
            //Arrange

            // Создаем экземпляр класса ExcelExport
            IExcelExport excelExport = new ExcelExport();

            // Определяем полный путь до формируемого excel-документа
            excelExport.SetFullPath("", "TestReport.xlsx");

            // Создаем и заполняем DataTable
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("FirstColumn");
            dataTable.Columns.Add("SecondColumn");
            dataTable.Columns.Add("FormulaColumn");

            dataTable.Rows.Add(new object[] { 1, 2, "=A2*B2" });
            dataTable.Rows.Add(new object[] { 2, 3, "=A3*B3" });
            dataTable.Rows.Add(new object[] { 3, 4, "=A4*B4" });

            //Act
            // Вызываем метод формирования отчета
            excelExport.ExportExcel(dataTable, "Тестовый отчет");

            //Assert
            // Проверяем, что по указаному пути был создан файл
            Assert.IsTrue(File.Exists("TestReport.xlsx"));

            // Удаляем созданный файл
            if (File.Exists("TestReport.xlsx"))
            {
                File.Delete("TestReport.xlsx");
            }            
        }
    }
}
