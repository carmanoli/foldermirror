using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace FolderMirror
{
    public class Logger
    {
        private string _logFile;

        public Logger(string logFile)
        {
            try
            {
                _logFile = Path.GetFullPath(logFile); 
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd-HHmmss}-logfile: {_logFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd-HHmmss}-Invalid logfile: {ex.Message}");
                throw;
            }
        }

        public void Log(object message)
        {
            String logLine;
            logLine = $"{DateTime.Now:yyyy-MM-dd-HHmmss}-{message}";
            Console.WriteLine(logLine);

            string folder = Path.GetDirectoryName(_logFile);

            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                try
                {
                    Directory.CreateDirectory(folder);
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd-HHmmss}-Created folder: {folder}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd-HHmmss}-Error creating folder: {ex.Message}");
                }
            }

            try
                {
                File.AppendAllText(_logFile, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                logLine = $"{DateTime.Now:yyyy-MM-dd-HHmmss}-Error writing logfile: {ex.Message}";
                Console.WriteLine(logLine);
            }
            

        }
    }
}
