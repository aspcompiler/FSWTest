using System;
using System.IO;

namespace FSWTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string tmpDir = Environment.GetEnvironmentVariable("TMPDIR")
                ?? Environment.GetEnvironmentVariable("TEMP")
                ?? Environment.GetEnvironmentVariable("TMP");

            if (string.IsNullOrEmpty(tmpDir)) throw new Exception("Please set environment variable TMPDIR or TEMP or TMP");

            string watchDir = Path.Combine(tmpDir, "FSWTest");
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string filePath = Path.Combine(watchDir, filename);

            PrepareDirectory(watchDir);
            CreateFiles(filePath);
            File.Delete(filePath);

            using(var watcher = CreateFileSystemWatcher(watchDir))
            {
                CreateFiles(filePath);
                Console.WriteLine("Press 'q' to quit the sample.");
                while (Console.Read() != 'q') { }
            }
        }

        private static void CreateFiles(string filePath)
        {
            File.WriteAllText(filePath, "something");
        }

        private static void PrepareDirectory(string watchDir)
        {
            if (Directory.Exists(watchDir))
                Directory.Delete(watchDir, true);

            Directory.CreateDirectory(watchDir);
        }

        private static FileSystemWatcher CreateFileSystemWatcher(string watchDir)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = watchDir;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.FileName
                                    | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private static void OnChanged(object source, FileSystemEventArgs e) =>
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");

        private static void OnRenamed(object source, RenamedEventArgs e) =>
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }
}
