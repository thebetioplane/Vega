using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using ManagedBass;

namespace Vega
{
    public static class Program
    {
        //public const string BUILD_ID = "20180108";
        public static bool NoNetwork = false;
        public static string[] RequiredFiles = { "Vega.exe", "OpenTK.dll", "ManagedBass.dll", "VegaImg.dat", "VegaSnd.dat" };
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            bool shouldUpdate = false;
            File.Delete(Maintenance.Updater.EXE_TMP_FILE);
            File.Delete(Maintenance.LocalFileIndex.SWAP_FILE_NAME);
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "no-update":
                        NoNetwork = true;
                        break;
                    case "force-update":
                        shouldUpdate = true;
                        Logger.DefaultLogger.WriteLine("Forcing update due to flag");
                        break;
                    case "make-index":
                    {
                        var localIndex = new Maintenance.LocalFileIndex(false);
                        localIndex.ToFile("index");
                        return;
                    }
                    default:
                        Logger.DefaultLogger.WriteLine("Unknown flag `{0}` not used");
                        break;
                }
            }
#if DEBUG
            Logger.DefaultLogger.WriteLine("Built with DEBUG configuration");
            NoNetwork = true;
#endif
            foreach (var requiredFile in RequiredFiles)
            {
                if (! File.Exists(requiredFile))
                {
                    Logger.DefaultLogger.WriteLine("Detected missing required file, `{0}`, forcing maintenance",
                        requiredFile);
                    shouldUpdate = true;
                    break;
                }
            }
            if (shouldUpdate)
            {
                if (NoNetwork)
                {
                    Logger.DefaultLogger.WriteLine("Networking disabled, failing maintenance");
                    MessageBox.Show("An update is required, but networking is disabled.", "Failed");
                    return;
                }
                Logger.DefaultLogger.WriteLine("Maintenance update started");
                Application.Run(new Maintenance.UpdaterGraphical());
                if (Maintenance.Updater.DefaultUpdater.Ready)
                {
                    Logger.DefaultLogger.WriteLine("Maintenance update succesful");
                    Program.Restart("no-update");
                }
                else
                {
                    // implies that the Maintenance window was closed early
                    Logger.DefaultLogger.WriteLine("Maintenence update interrupted");
                }
                return;
            }
            try
            {
                EntryPoint();
            }
            catch (System.IO.FileNotFoundException e)
            {
                Logger.DefaultLogger.WriteLine("File not found exception, assuming missing asset");
                Logger.DefaultLogger.WriteError(e);
                DoUpdateRetryDialog("Missing Asset", string.Format("Vega could not find the file `{0}`, possibly do to a failed update.\n\nPress OK to force an update.", e.FileName));
            }
            catch (DllNotFoundException e)
            {
                Logger.DefaultLogger.WriteLine("Detected missing .dll `{0}`", e.Message);
                Logger.DefaultLogger.WriteError(e);
                DoUpdateRetryDialog("Missing .DLL", string.Format("The .dll `{0}` was not found. OK to force an update ?", e.Message));
            }
#if !DEBUG
            catch (Exception e)
            {
                Logger.DefaultLogger.WriteLine("FATAL");
                Logger.DefaultLogger.WriteError(e);
                DoRetryDialog("Useless Error Handler", string.Format("{0}\n{1}\n\nError details included in file `{2}`, include it in your crash report.",
                    e.GetType(), e.Message, Logger.DefaultLogger.FileName));
            }
#endif
            finally
            {
                Logger.DefaultLogger.WriteLine("Program exited");
                Logger.DefaultLogger.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void EntryPoint()
        {
            using (Main main = new Main())
            {
                main.Run(60.0, 0.0);
            }
        }

        private static void DoRetryDialog(string title, string message)
        {
            DialogResult res = MessageBox.Show(message, title, MessageBoxButtons.RetryCancel);
            if (res == DialogResult.Retry)
            {
                Program.Restart();
            }
        }

        private static void DoUpdateRetryDialog(string title, string message)
        {
            DialogResult res = MessageBox.Show(message, title, MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                File.Delete(Maintenance.LocalFileIndex.FILE_NAME);
                Program.Restart("force-update");
            }
        }

        public static bool Restart(string args = "")
        {
            Logger.DefaultLogger.WriteLine("Restarting with flags = `{0}`", args);
            try
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath, args);
                return true;
            }
            catch (Exception e)
            {
                Logger.DefaultLogger.WriteLine("Restart failed");
                Logger.DefaultLogger.WriteError(e);
                return false;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = args.ExceptionObject as Exception;
            Logger.DefaultLogger.WriteLine("UNHANDLED EXCEPTION");
            if (e == null)
                Logger.DefaultLogger.WriteLine(args.ExceptionObject.ToString());
            else
                Logger.DefaultLogger.WriteError(e);
        }
    }
}
