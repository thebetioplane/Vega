using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Vega.Maintenance
{
    public delegate void TextChangedHandler(object sender, string value);
    public delegate void PercentChangedHandler(object sender, int value);
    public class Updater : IDisposable
    {
        public const string TMP_DIR = ".tmp";
        public static Updater DefaultUpdater = new Updater();
        private readonly string TIMESTAMP = "?t=" + DateTime.Now.Ticks.ToString();
        private WebClient Client;
        private Logger Log = new Logger("updater.log");
        public Updater(bool ready)
        {
            this.Ready = ready;
        }
        public bool Ready { get; private set; }

        public event TextChangedHandler TextChanged;
        public event PercentChangedHandler PercentChanged;
        private Updater()
        {
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += (sender, data) =>
            {
                if (this.PercentChanged != null)
                    this.PercentChanged(this, data.ProgressPercentage);
            };
            this.Ready = true;
        }
        /// <summary>
        /// Runs the updater
        /// </summary>
        /// <param name="fullUpdate">true if the updater should ignore the local cache</param>
        /// <returns>true when successful</returns>
        public bool Run(bool fullUpdate)
        {
            if (!this.Ready)
                throw new InvalidOperationException("The updater is still busy");
            if (Program.NoNetwork)
                throw new InvalidOperationException("Networking is disabled");
            this.Ready = false;
            if (this.PercentChanged != null)
                this.PercentChanged(this, 0);
            if (this.TextChanged != null)
                this.TextChanged(this, "Getting file index");
            this.Log.WriteLine("Getting file index");
            try
            {
                var localIndex = new LocalFileIndex(! fullUpdate);
                this.Client.DownloadFile(this.GetWebUrl("index"), LocalFileIndex.SWAP_FILE_NAME);
                var index = new FileIndex();
                index.FromFile(LocalFileIndex.SWAP_FILE_NAME);
                var filesToGet = new List<string>();
                foreach (var item in index)
                {
                    if (localIndex[item.Key] == null || ! localIndex[item.Key].Equals(item.Value))
                        filesToGet.Add(item.Key);
                }
                this.Log.WriteLine("{0} files are missing or outdated", filesToGet.Count);
                if (filesToGet.Count == 0)
                {
                    if (this.TextChanged != null)
                        this.TextChanged(this, "Vega is up to date");
                    this.Ready = true;
                    return true;
                }
#if INSTALL_WARNING
                if (filesToGet.Count >= index.Count - 1)
                {
                    System.Windows.Forms.Application.Run(new InstallWarning());
                }
#endif
                Directory.CreateDirectory(TMP_DIR);
                foreach (var file in filesToGet)
                {
                    string status = "Downloading " + file;
                    this.Log.WriteLine(status);
                    if (this.TextChanged != null)
                        this.TextChanged(this, status);
                    var dlFile = Path.Combine(TMP_DIR, file + ".dl");
                    var trFile = Path.Combine(TMP_DIR, file + ".trash");
                    this.Client.DownloadFile(this.GetWebUrl(file), dlFile);
                    if (File.Exists(file))
                        File.Move(file, trFile);
                    File.Move(dlFile, file);
                }
                if (this.TextChanged != null)
                    this.TextChanged(this, "Update finished, restart to apply");
            }
            catch (WebException e)
            {
                this.Log.WriteLine("Failed: No internet connection?");
                this.Log.WriteError(e);
                Program.NoNetwork = true;
                if (this.TextChanged != null)
                    this.TextChanged(this, "There is no internet connection");
                File.Delete(LocalFileIndex.SWAP_FILE_NAME);
                return false;
            }
            finally
            {
                if (File.Exists(LocalFileIndex.SWAP_FILE_NAME))
                {
                    File.Delete(LocalFileIndex.FILE_NAME);
                    File.Move(LocalFileIndex.SWAP_FILE_NAME, LocalFileIndex.FILE_NAME);
                }
                this.Log.WriteLine("Updater finished");
                this.Log.WriteLine("-------------------");
            }
            this.Ready = true;
            return true;
        }

        private string GetWebUrl(string relPath)
        {
            return "https://raw.githubusercontent.com/thebetioplane/Vega/master/distro/" + relPath + TIMESTAMP;
        }

        public void Dispose()
        {
            this.Client?.Dispose();
            this.Log?.Dispose();
        }
    }
}