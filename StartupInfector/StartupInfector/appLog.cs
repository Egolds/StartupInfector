using System;
using System.IO;
using System.Text;

namespace StartupInfector
{
    class appLog
    {
        public void Write(string inputText, bool newLine = false)
        {
            if (newLine)
                Console.WriteLine();

            Console.WriteLine(inputText);
            saveLog(inputText, newLine);
        }

        public void Write(object inputText, bool newLine = false)
        {
            if (newLine)
                Console.WriteLine();

            Console.WriteLine(inputText);
            saveLog(inputText.ToString(), newLine);
        }

        private void saveLog(string inputText, bool newLine = false)
        {
            string textLog = $" {inputText}\r\n";

            if (newLine)
            {
                string time = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] ";
                textLog = "\r\n" + time + "\r\n" + textLog;
            }

            string log_file_path = Path.Combine(Path.GetDirectoryName(appConfing.MyFullPath), "Log.txt");

            using (FileStream fs = new FileStream(log_file_path, FileMode.Append))
            {
                byte[] bytes = Encoding.Default.GetBytes(textLog);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
