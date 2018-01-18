using System;
using System.IO;
using System.Collections.Generic;

namespace Vega
{
    public class AssetMap : IDisposable
    {
        private FileStream FStream;
        private BinaryReader Reader;
        private Dictionary<string, long> Locations;
        public int Count { get; private set; }
        private string AsmName;
        public AssetMap(string fileName)
        {
            this.AsmName = fileName;
            this.FStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            this.Reader = new BinaryReader(this.FStream);
            this.Locations = new Dictionary<string, long>();
            this.Count = this.Reader.ReadInt32();
            var names = new string[this.Count];
            for (int i = 0; i < this.Count; ++i)
            {
                names[i] = this.Reader.ReadString();
            }
            for (int i = 0; i < this.Count; ++i)
            {
                this.Locations.Add(names[i], this.Reader.ReadInt64());
            }
        }
        public Stream GetAsset(string fileName)
        {
            if (! this.Locations.TryGetValue(fileName, out long position))
                throw new FileNotFoundException(string.Format("`{0}` not found in `{1}`",
                    fileName, this.AsmName), fileName);
            this.FStream.Position = position;
            int bsize = this.Reader.ReadInt32();
            MemoryStream ms = new MemoryStream();
            this.FStream.CopyTo(ms, bsize);
            return ms;
            //return this.FStream;
        }
        public void Dispose()
        {
            this.Reader?.Dispose();
            this.FStream?.Dispose();
        }
    }
}