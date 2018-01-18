using System;
using OpenTK.Graphics.OpenGL;

namespace Vega.GameModePlay
{
    public class LevelStage : IDisposable
    {
        private readonly string Path;
        public StageScript.StageScript Script { get; private set; }
        public Texture2D[] BulletTexture { get; private set; }
        public Texture2D[] EnemyTexture { get; private set; }
        public Texture2D[] BackgroundTexture { get; private set; }
        public SoundEffect[] Sounds { get; private set; }
        public int[] EnemyTextureWidth { get; private set; }
        public int[] EnemyTextureFrame { get; private set; }

        public LevelStage(string path)
        {
            this.Path = path;
            this.Script = new StageScript.StageScript(this.PathCombine("ssf"));
            IniFile index = new IniFile(this.PathCombine("index.ini"));
            index.ReadFile();
            int soundCount = Convert.ToInt32(index["sounds"]);
            this.Sounds = new SoundEffect[soundCount];
            for (int i = 0; i < soundCount; i++)
                this.Sounds[i] = new SoundEffect(this.PathCombine(string.Format("s_{0}.wav", i)));
            int backgrounds = Convert.ToInt32(index["backgrounds"]);
            this.BackgroundTexture = new Texture2D[backgrounds];
            for (int i = 0; i < backgrounds; i++)
                this.BackgroundTexture[i] = new Texture2D(this.PathCombine(string.Format("bg_{0}.png", i)));
            int bullets = Convert.ToInt32(index["bullets"]);
            this.BulletTexture = new Texture2D[bullets];
            for (int i = 0; i < bullets; i++)
                this.BulletTexture[i] = new Texture2D(this.PathCombine(string.Format("b_{0}.png", i)));
            int enemies = Convert.ToInt32(index["enemies"]);
            this.EnemyTexture = new Texture2D[enemies];
            for (int i = 0; i < enemies; i++)
                this.EnemyTexture[i] = new Texture2D(this.PathCombine(string.Format("e_{0}.png", i)));
            this.EnemyTextureWidth = new int[enemies];
            this.EnemyTextureFrame = new int[enemies];
            IniFile anim = new IniFile(this.PathCombine("anim.ini"));
            anim.ReadFile();
            for (int i = 0; i < enemies; i++)
            {
                string val = anim[string.Format("e_{0}", i)];
                if (val == null)
                {
                    throw new StageScript.StageScriptException("You did not specify a sprite width and frame count for e_" + i);
                }
                else
                {
                    string[] splt = val.Split(',');
                    this.EnemyTextureWidth[i] = Convert.ToInt32(splt[0]);
                    this.EnemyTextureFrame[i] = Convert.ToInt32(splt[1]);
                }
            }
        }

        private string PathCombine(string path)
        {
            return System.IO.Path.Combine(this.Path, path);
        }

        public void Dispose()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            foreach (Texture2D texture in this.EnemyTexture)
                texture.Dispose();
            foreach (Texture2D texture in this.BulletTexture)
                texture.Dispose();
            foreach (Texture2D texture in this.BackgroundTexture)
                texture.Dispose();
            foreach (SoundEffect sound in this.Sounds)
                sound.Dispose();
        }
    }
}
