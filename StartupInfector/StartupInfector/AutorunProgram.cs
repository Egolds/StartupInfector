namespace StartupInfector
{
    class AutorunProgram
    {
        /// <summary>
        /// Название элемента в разделе реестра
        /// </summary>
        public string RegName { get; set; }

        /// <summary>
        /// Значение элемента в разделе реестра, может содержать путь к файлу и аргументы запуска
        /// </summary>
        public string RegValue { get; set; }


        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string RunFilePath { get; set; }

        /// <summary>
        /// Аргументы запуска файла
        /// </summary>
        public string RunArguments { get; set; }

        
        /// <summary>
        /// Существует ли файл
        /// </summary>
        public bool IsFileExists { get; set; } = false;

        /// <summary>
        /// Запущен ли процесс файла
        /// </summary>
        public bool IsActiveProcess { get { return cProcesses.IsFileActiveProcess(RunFilePath); } }

        /// <summary>
        /// Базовый раздел реестра в котором находится этот элемент
        /// </summary>
        public RegistryLocation RegLocation { get; set; }
        public enum RegistryLocation { LocalMachine, CurrentUser };
    }
}
