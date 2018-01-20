using System;
using System.IO;

namespace Vega.GameModePlay
{
    public class Level : IDisposable
    {
        private readonly string Path;
        public string Title { get; private set; }
        private LevelStage[] Stages;
        private int Selected = 0;
        public LevelStage CurrentStage { get { return this.Stages[this.Selected]; } }
        public StageScript.StageScript Script { get { return this.CurrentStage.Script; } }

        public Level(string path)
        {
            try
            {
                this.Path = path;
                IniFile index = new IniFile(System.IO.Path.Combine(this.Path, "index.ini"));
                index.ReadFile();
                this.Title = index["title"];
                int n = Convert.ToInt32(index["stages"]);
                this.Stages = new LevelStage[n];
                for (int i = 0; i < n; i++)
                    this.Stages[i] = new LevelStage(this.PathCombine(i.ToString()));
            }
            catch (FileNotFoundException e)
            {
                throw new StageScript.StageScriptException(e.Message);
            }
        }

        private string PathCombine(string path)
        {
            return System.IO.Path.Combine(this.Path, path);
        }

        public void SelectStage(int num)
        {
            this.Selected = num;
        }

        public void Dispose()
        {
            foreach (LevelStage stage in this.Stages)
                stage.Dispose();
        }
    }
}
