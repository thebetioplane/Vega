using System;
using System.IO;

namespace Vega.Maintenance
{
    public class LocalFileIndex : FileIndex
    {
        public const string FILE_NAME = "updater.cache";
        public const string SWAP_FILE_NAME = FILE_NAME + ".swp";
        public LocalFileIndex(bool useCache)
        {
            if (useCache && File.Exists(FILE_NAME))
            {
                try
                {
                    this.FromFile(FILE_NAME);
                    return;
                }
                catch
                {
                }
            }
            foreach (var file in Program.RequiredFiles)
            {
                if (! File.Exists(file))
                    continue;
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    this[file] = new MD5Sum(fs);
                }
            }
        }
    }
}