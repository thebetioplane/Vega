using System;
using System.IO;

namespace Vega
{
    public class Logger : IDisposable
    {
        public static Logger DefaultLogger = new Logger("vega.log");

        public string FileName { get; private set; }
        public bool Enabled { get; private set; }
        public bool IsDisposed { get; private set; }
        private StreamWriter writer;
        private DateTime logStartTime;

        public Logger(string filename)
        {
            this.FileName = filename;
            try
            {
                this.writer = new StreamWriter(new FileStream(this.FileName, FileMode.Create, FileAccess.Write));
            }
            catch
            {
                this.writer = null;
            }
            this.IsDisposed = false;
            this.Enabled = this.writer != null;
            this.logStartTime = DateTime.UtcNow;
            this.WriteLine("`{0}` on {1}", this.FileName, Environment.OSVersion);
            this.WriteLine("Current UTC Timestamp : [{0}] {1}", this.logStartTime.ToString("yyyy-MM-dd HH:mm:ss"), this.logStartTime.Ticks);
            this.WriteLine("==============================");
        }

        public void WriteLine(string fmt, params object[] obj)
        {
            if (!this.Enabled)
                return;
            TimeSpan diff = DateTime.UtcNow - this.logStartTime;
            string timestamp = string.Format("[{0:d2}.{1:d2}.{2:d2}] ", diff.Hours, diff.Minutes, diff.Seconds);
            string message = timestamp + string.Format(fmt, obj);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
            this.writer.WriteLine(message);
            this.writer.Flush();
        }

        public void WriteError(Exception e)
        {
            this.WriteLine(e.ToString());
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            if (this.writer != null)
                this.writer.Dispose();
            this.IsDisposed = true;
        }
    }
}
