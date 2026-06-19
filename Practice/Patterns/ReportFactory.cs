using System;
using System.IO;

namespace Practice.Patterns
{
    public abstract class BaseReport
    {
        public abstract bool Export(string path, string docName, string content);
    }

    public class PdfAnalyticReport : BaseReport
    {
        public override bool Export(string path, string docName, string content)
        {
            try
            {
                int totalChars = content.Length;
                int wordCount = content.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int lineCount = content.Split(new char[] { '\n' }).Length;

                string htmlTemplate = $@"
                <html>
                <head><meta charset='utf-8'><style>body {{ font-family: 'Arial'; margin: 40px; }} h1 {{ border-bottom: 2px solid #2c3e50; }} table {{ width: 100%; border-collapse: collapse; }} td, th {{ border: 1px solid #ddd; padding: 8px; }}</style></head>
                <body>
                    <h1>РЕГЛАМЕНТИРОВАННЫЙ ОТЧЕТ (PDF)</h1>
                    <p><b>Документ:</b> {docName}</p>
                    <p><b>Дата:</b> {DateTime.Now}</p>
                    <hr/>
                    <h3>АНАЛИТИЧЕСКАЯ ТАБЛИЦА:</h3>
                    <table>
                        <tr><th>Метрика текста</th><th>Значение</th></tr>
                        <tr><td>Количество строк</td><td>{lineCount}</td></tr>
                        <tr><td>Количество слов</td><td>{wordCount}</td></tr>
                        <tr><td>Количество символов</td><td>{totalChars}</td></tr>
                    </table>
                    <h3>СОДЕРЖИМОЕ:</h3>
                    <div style='background: #f9f9f9; padding: 10px; border: 1px solid #ccc;'>{System.Net.WebUtility.HtmlEncode(content)}</div>
                </body>
                </html>";

                string tempHtml = Path.Combine(Path.GetTempPath(), "factory_temp.html");
                File.WriteAllText(tempHtml, htmlTemplate, System.Text.Encoding.UTF8);

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/c start msedge --headless --print-to-pdf=\"{path}\" \"{tempHtml}\"");
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process.Start(psi).WaitForExit(3000);

                return true;
            }
            catch { return false; }
        }
    }

    public class ExcelCsvReport : BaseReport
    {
        private bool _isExcelFormat;

        public ExcelCsvReport(bool isExcelFormat)
        {
            _isExcelFormat = isExcelFormat;
        }

        public override bool Export(string path, string docName, string content)
        {
            try
            {
                int totalChars = content.Length;
                int wordCount = content.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int lineCount = content.Split(new char[] { '\n' }).Length;

                string csvContent = "Metric;Value\n" +
                                   $"Document Name;{docName}\n" +
                                   $"Lines Count;{lineCount}\n" +
                                   $"Words Count;{wordCount}\n" +
                                   $"Characters Count;{totalChars}\n";

                File.WriteAllText(path, csvContent, System.Text.Encoding.UTF8);
                return true;
            }
            catch { return false; }
        }
    }

    public class ReportFactory
    {
        public static BaseReport CreateReport(string reportType)
        {
            switch (reportType.ToUpper())
            {
                case "PDF":
                    return new PdfAnalyticReport();
                case "EXCEL":
                    return new ExcelCsvReport(true);
                case "CSV":
                    return new ExcelCsvReport(false);
                default:
                    throw new ArgumentException("Неизвестный тип отчета");
            }
        }
    }
}