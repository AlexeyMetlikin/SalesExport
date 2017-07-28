using System.Data;

namespace SalesReport.Models.Abstract
{
    public abstract class IExcelExport
    {
        public string FullPath { get; private set; } // Полный путь до сформированного Excel-отчета

        // Метод формирования отчета
        public abstract void ExportExcel(DataTable dataTable, string heading = "", int colHeaderRows = 1);

        // Метод определения полного пути
        public void SetFullPath(string path, string fileName)
        {
            FullPath = System.IO.Path.Combine(path, fileName);  // Формируем путь
            if (System.IO.File.Exists(FullPath))                // Если файл существует
            {
                string extension = System.IO.Path.GetExtension(FullPath);
                fileName = System.IO.Path.GetFileNameWithoutExtension(FullPath);
                int i = 1;
                while (System.IO.File.Exists(FullPath))
                {
                    FullPath = System.IO.Path.Combine(path, fileName + " (" + i.ToString() + ")" + extension); // Добавляем к файлу номер: (1), (2), ..., (n)
                    i++;
                }
            }
        }
    }
}