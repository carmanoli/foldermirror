using FolderMirror;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderMirror
{

    public record Settings
    {
        public string? Source { get; set; }
        public string? Target { get; set; }
        public string? LogFile { get; set; } = $"{DateTime.Now:yyyy-MM-dd-HHmmss}-FileMirror.log";
        public int? Interval { get; set; } = 60;
    }
        public class SetSettings
    {

        public Settings settings { get; }
        public SetSettings(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddCommandLine(args);
            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            var _settings = new Settings();
            configurationRoot.Bind(_settings);

            if (!string.IsNullOrEmpty(_settings.LogFile) )
            {
                if (!Path.IsPathRooted(_settings.LogFile))
                {
                    _settings.LogFile = Path.Combine(Directory.GetCurrentDirectory(), _settings.LogFile);
                }
            }
            else
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd-HHmmss}-logfile is empty: {_settings.LogFile}");
            }
            settings = _settings;
        }

    }
}
