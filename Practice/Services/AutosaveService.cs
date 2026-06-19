using System;
using System.IO;

namespace Practice.Services
{
    public class AutosaveService
    {
        private string _backupPath;

        public AutosaveService()
        {
            _backupPath = Path.Combine(Path.GetTempPath(), "PracticeAutosave.txt");
        }

        public void SaveBackup(string content)
        {
            try
            {
                File.WriteAllText(_backupPath, content, System.Text.Encoding.UTF8);
            }
            catch { }
        }
    }
}