
using FolderMirror;
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace FolderMirror
{
    public class Program
    {



        public static async Task Main(string[] args)
        {
            var setSettings = new SetSettings(args);
            var settings = setSettings.settings;

            var logger = new Logger(settings!.LogFile!);
            var fileMirror = new FileMirror(logger);

            logger.Log("--------------------------------------------------------------------------------");
            logger.Log("Starting folder mirror.");

            logger.Log(settings);
            fileMirror.startFolderMirror(settings!.Source!, settings!.Target!);

            logger.Log("Ending folder mirror.");
            logger.Log("--------------------------------------------------------------------------------");

        }
    }
}