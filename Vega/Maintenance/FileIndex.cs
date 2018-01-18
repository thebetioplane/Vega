using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using KVPair = System.Collections.Generic.KeyValuePair<string, Vega.Maintenance.MD5Sum>;

namespace Vega.Maintenance
{
    public class FileIndex : IEnumerable<KVPair>
    {
        private Dictionary<string, MD5Sum> Backing;
        public FileIndex() => this.Backing = new Dictionary<string, MD5Sum>();
        public int Count => this.Backing.Count;
        public void FromFile(string fname)
        {
            using (var fstream = new FileStream(fname, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fstream))
            {
                int n = reader.ReadInt32();
                for (int i = 0; i < n; ++i)
                {
                    string name = reader.ReadString();
                    MD5Sum sum = reader.ReadBytes(16);
                    this.Backing.Add(name, sum);
                }
            }
        }
        public void ToFile(string fname)
        {
            using (var fstream = new FileStream(fname, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fstream))
            {
                writer.Write(this.Count);
                foreach (var item in this.Backing)
                {
                    writer.Write(item.Key);
                    writer.Write(item.Value);
                }
            }
        }
        public MD5Sum this[string key]
        {
            get => (this.Backing.ContainsKey(key)) ? this.Backing[key] : null;
            set => this.Backing.Add(key, value);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public IEnumerator<KVPair> GetEnumerator()
        {
            return this.Backing.GetEnumerator();
        }
    }
}