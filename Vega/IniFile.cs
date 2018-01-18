using System.IO;
using System.Collections.Generic;

namespace Vega
{
    public class IniFile
    {
        public string FileName { get; private set; }
        private Dictionary<string, string> Dict;

        public IniFile(string filename)
        {
            this.FileName = filename;
            this.Dict = new Dictionary<string, string>();
        }

        public void ReadFile()
        {
            string[] lines = File.ReadAllLines(this.FileName);
            foreach (string line in lines)
            {
                if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                    continue;
                string[] split = line.Split('=');
                string s2 = (split.Length == 2) ? split[1] : string.Empty;
                this.Dict.Add(split[0], s2);
            }
        }

        public string this[string key]
        {
            get
            {
                if (this.Dict.ContainsKey(key))
                    return this.Dict[key];
                else
                    return null;
            }
            set
            {
                this.Dict[key] = value;
            }
        }

        public void WriteFile()
        {
            using (FileStream fs = new FileStream(this.FileName, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                foreach (KeyValuePair<string, string> kv in this.Dict)
                    writer.WriteLine("{0}={1}", kv.Key, kv.Value);
            }
        }
    }
}
