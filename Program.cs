
using FolderMirror;
using System;
using System.Diagnostics.Tracing;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace FolderMirror
{
    public class Program
    {

        private static CancellationTokenSource _CtrlC = new CancellationTokenSource();

        public static async Task Main(string[] args)
        {
            var setSettings = new SetSettings(args);
            var settings = setSettings.settings;
            if (!setSettings.ValidSettings)
            {
                return; 
            }

            var logger = new Logger(settings!.LogFile!);
            var fileMirror = new FileMirror(logger);

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CtrlCHandler);

            try
            {
                while (!_CtrlC.Token.IsCancellationRequested)
                {
                    logger.Log("--------------------------------------------------------------------------------");
                    logger.Log("Starting folder mirror.");
                    logger.Log("Press Ctrl+C to cancel.");
                    logger.Log(settings);
                    fileMirror.startFolderMirror(settings!.Source!, settings!.Target!);
                    logger.Log("Ending folder mirror.");
                    logger.Log("--------------------------------------------------------------------------------");
                    logger.Log($"Waiting for {settings.Interval.Value} seconds...");
                    await Task.Delay(settings.Interval.Value * 1000, _CtrlC.Token);
                }
            }
            catch (Exception ex)
            {
                logger.Log($"{ex.Message}");
                return;
            }
        }

        protected static void CtrlCHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            _CtrlC.Cancel();
        }
    }
}