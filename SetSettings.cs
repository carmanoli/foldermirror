using FolderMirror;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FolderMirror
{
    public record Settings
    {
        public string? Source { get; set; }
        public string? Target { get; set; }
        public string? LogFile { get; set; } = $"{DateTime.Now:yyyy-MM-dd-HHmmss}-FileMirror.log";
        public int? Interval { get; set; } = 10;
    }
    public class SetSettings
    {
        public bool ValidSettings { get; private set; }

        public Settings settings { get; }
        public SetSettings(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddCommandLine(args);
            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            settings = new Settings();
            configurationRoot.Bind(settings);

            ValidateSettngs();
        }
        private void ValidateSettngs()
        {
            ValidSettings = true;
            if (string.IsNullOrWhiteSpace(settings.Source))
            {
                Console.WriteLine("Parameter --scource is missing.");
                ValidSettings = false;
            }
            if (string.IsNullOrWhiteSpace(settings.Target))
            {
                Console.WriteLine("Parameter --target is missing.");
                ValidSettings = false;
            }
            if (!string.IsNullOrWhiteSpace(settings.Source) && !string.IsNullOrWhiteSpace(settings.Target))
            {
                if (settings.Source == settings.Target)
                {
                    Console.WriteLine("ERRO: A pasta de origem e destino não podem ser a mesma.");
                    ValidSettings = false;
                }
            }
            if (settings.Source != null && !Directory.Exists(settings.Source))
            {
                Console.WriteLine($"Source folder not found: '{settings.Source}'");
                ValidSettings = false;
            }
            if (settings.Interval == null || settings.Interval < 1)
            {
                Console.WriteLine($"Invalid --interval: '{settings.Interval}'");
                ValidSettings = false;
            }

            if (string.IsNullOrWhiteSpace(settings.LogFile))
            {
                Console.WriteLine("Parameter --logfile is missing.");
                ValidSettings = false;
            }
            else
            {
                if (!Path.IsPathRooted(settings.LogFile))
                {
                    settings.LogFile = Path.Combine(Directory.GetCurrentDirectory(), settings.LogFile);
                }
                if (!Directory.Exists(Path.GetDirectoryName(settings.LogFile)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(settings.LogFile)!);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating log folder: {ex.Message}");
                        ValidSettings = false;
                    }
                }
            }

            if (!ValidSettings)
            {
                ShowCommandLineParameters();
            }
        }
        private static void ShowCommandLineParameters()
        {
            Console.WriteLine("foldermirror syntax:");
            Console.WriteLine("foldermirror --source <source_folder> --target <destination_folder> [--logfile <lofile_name_>] [--interval <seconds>]");
        }
    }
}