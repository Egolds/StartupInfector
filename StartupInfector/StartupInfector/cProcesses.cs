using System;
using System.Diagnostics;
using System.IO;

namespace StartupInfector
{
    class cProcesses
    {
        public static bool IsFileActiveProcess(string pathFile)
        {
            string processName = string.Empty;

            // Получаем название файла без расширения по заданному пути к файлу
            processName = Path.GetFileNameWithoutExtension(pathFile);

            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                if (proc.MainModule.FileName == pathFile)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool KillFileProcess(string pathFile, bool killAllCopy = false)
        {
            string processName = string.Empty;

            // Получаем название файла без расширения по заданному пути к файлу
            processName = Path.GetFileNameWithoutExtension(pathFile);

            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                if (!killAllCopy)
                {
                    if (proc.MainModule.FileName == pathFile)
                    {
                        proc.Kill();
                        return true;
                    }
                }
                else
                {
                    proc.Kill();
                    return true;
                }
            }

            return false;
        }
    }
}
