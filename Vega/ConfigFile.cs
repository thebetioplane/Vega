using System;
using System.IO;

namespace Vega
{
    public class ConfigFile
    {
        public string FileName { get; private set; }
        private IniFile Core;
        public ConfigFile()
        {
            this.FileName = "config.ini";
            this.Core = new IniFile(this.FileName);
            if (File.Exists(this.FileName))
                this.Core.ReadFile();
            this.Save();
        }

        private string CheckParam(string key, string defaultValue)
        {
            if (this.Core[key] == null)
            {
                this.Core[key] = defaultValue;
                return defaultValue;
            }
            else
            {
                return this.Core[key];
            }
            
        }

        private int CheckParam(string key, int defaultValue)
        {
            if (this.Core[key] == null)
            {
                this.Core[key] = defaultValue.ToString();
                return defaultValue;
            }
            else
            {
                int result;
                if (Int32.TryParse(this.Core[key], out result))
                    return result;
                else
                    return defaultValue;
            }
        }

        public void Save()
        {
            this.Core.WriteFile();
        }
    }
}
