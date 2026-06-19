using System;
using System.Windows.Forms;
using Practice.Controllers;
using Practice.Views;
using Practice.Patterns;

namespace Practice
{
    public partial class Form1 : Form
    {
        private readonly DocumentController _documentController = new DocumentController();
        private int _documentCount = 0;
        private TextProcessor _textProcessor = new TextProcessor();

        public Form1()
        {
            InitializeComponent();

            _textProcessor.SetStrategy(new UpperCaseStrategy());

            if (LoginForm.UserRole == "User")
            {
                this.отчетыToolStripMenuItem.Enabled = false;
                this.Text += " (Режим: Пользователь)";
                toolStripStatusLabel1.Text = "Пользователь: User | Программа готова к работе";
            }
            else if (LoginForm.UserRole == "Admin")
            {
                this.отчетыToolStripMenuItem.Enabled = true;
                this.Text += " (Режим: Администратор)";
                toolStripStatusLabel1.Text = "Пользователь: Admin | Доступен полный функционал";
            }
        }

        private void создатьДокументToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _documentCount++;

            DocumentChildForm childForm = new DocumentChildForm();
            childForm.MdiParent = this;
            childForm.Text = $"Новый документ {_documentCount}";

            childForm.ContentChangedObserver += (childSender, currentText) => {
                int words = currentText.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                toolStripStatusLabel1.Text = $"Редактирование: Слов: {words} | Символов: {currentText.Length}";
            };

            childForm.Show();
        }

        private void сохранитьДокументToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentChildForm activeChild = this.ActiveMdiChild as DocumentChildForm;
            if (activeChild == null) return;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Файлы документов (*.json)|*.json|Все файлы (*.*)|*.*";
                saveFileDialog.FileName = activeChild.Text.Replace("*", "").Trim();

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fullPath = saveFileDialog.FileName;
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                        string content = activeChild.DocumentText;

                        string jsonText = $"{{\n  \"Title\": \"{fileName}\",\n  \"Content\": \"{content.Replace("\"", "\\\"").Replace("\n", "\\n")}\"\n}}";
                        System.IO.File.WriteAllText(fullPath, jsonText, System.Text.Encoding.UTF8);

                        _documentController.AddDocument(fileName, content, "Общие");

                        MessageBox.Show("Документ успешно сохранен на диск!", "Успех");
                        activeChild.MarkAsSaved(fileName);
                        toolStripStatusLabel1.Text = $"Успешно сохранен файл: {fileName}.json";
                    }
                    catch { }
                }
            }
        }

        private void экспортВPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentChildForm activeChild = this.ActiveMdiChild as DocumentChildForm;
            if (activeChild == null) return;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Документ PDF (*.pdf)|*.pdf|Книга Excel (*.xls)|*.xls|Файл CSV (*.csv)|*.csv";
                saveFileDialog.FileName = "Отчет_" + activeChild.Text.Replace("*", "").Trim();

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string docName = activeChild.Text.Replace("*", "");
                    string formattedText = _textProcessor.Execute(activeChild.DocumentText);

                    string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToUpper();
                    BaseReport report;

                    if (extension == ".XLS") report = ReportFactory.CreateReport("EXCEL");
                    else if (extension == ".CSV") report = ReportFactory.CreateReport("CSV");
                    else report = ReportFactory.CreateReport("PDF");

                    bool success = report.Export(saveFileDialog.FileName, docName, formattedText);

                    if (success)
                    {
                        MessageBox.Show("Отчет успешно сгенерирован!", "Успех");
                        toolStripStatusLabel1.Text = $"Сгенерирован отчет: {System.IO.Path.GetFileName(saveFileDialog.FileName)}";
                    }
                }
            }
        }

        private void открытьДокументToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы JSON (*.json)|*.json|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string content = System.IO.File.ReadAllText(openFileDialog.FileName);
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                        DocumentChildForm childForm = new DocumentChildForm();
                        childForm.MdiParent = this;
                        childForm.OpenExistingDocument(fileName, content);

                        childForm.ContentChangedObserver += (childSender, currentText) => {
                            int words = currentText.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                            toolStripStatusLabel1.Text = $"Редактирование: Слов: {words} | Символов: {currentText.Length}";
                        };

                        childForm.Show();
                        toolStripStatusLabel1.Text = $"Успешно открыт файл: {fileName}";
                    }
                    catch { }
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (this.ActiveMdiChild as DocumentChildForm)?.UndoAction();
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (this.ActiveMdiChild as DocumentChildForm)?.RedoAction();
        }

        private void статистикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (this.ActiveMdiChild as DocumentChildForm)?.ToggleGridVisibility();
        }
    }
}