using OfficeOpenXml;
using OfficeOpenXml.Style;
using SalesReport.Models.Abstract;
using System.Data;
using System.IO;

namespace SalesReport.Models.Context
{
    public class ExcelExport : IExcelExport
    {
        // Переопределение метода формирования отчета
        public override void ExportExcel(DataTable dataTable, string heading = "", int colHeaderRows = 1)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(heading);    // heading - Заголовок для листа в Excel

                // add the content into the Excel file  
                workSheet.Cells["A1"].LoadFromDataTable(dataTable, true);   // Загружаем в Excel данные из dataTable

                for (int i = colHeaderRows + 1; i <= colHeaderRows + dataTable.Rows.Count; i++)
                {
                    for (int j = 1; j <= dataTable.Columns.Count; j++)
                    {
                        if (dataTable.Rows[i - (colHeaderRows + 1)][j - 1].ToString()[0] == '=')    // Если какая-то поле в dataTable вычисляемое
                        {
                            // Инициализируем формулу для данной ячейки в Excel и устанавливаем для нее формат
                            workSheet.Cells[i, j].Formula = dataTable.Rows[i - (colHeaderRows + 1)][j - 1].ToString();
                            workSheet.Cells[i, j].Style.Numberformat.Format = "#,##0.00";
                        }
                    }
                }

                // Авторазмер колонок (под содержимое)
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    workSheet.Column(columnIndex).AutoFit();
                    columnIndex++;
                }

                // Стиль для заголовка таблицы - жирный текст, выравнивание по центру, заливка серого цвета
                using (ExcelRange r = workSheet.Cells[colHeaderRows, 1, colHeaderRows, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Font.Bold = true;
                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#cccccc"));
                }

                // Стиль для ячеек с данными - черная рамка по периметру 
                using (ExcelRange r = workSheet.Cells[colHeaderRows + 1, 1, dataTable.Rows.Count + colHeaderRows, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                package.SaveAs(new FileInfo(FullPath)); // Сохраняем документ по пути FullPath
            }
        }
    }
}