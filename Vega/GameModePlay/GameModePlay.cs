using System;

namespace Vega.GameModePlay
{
    public class GameModePlay : GameMode
    {
        private Level Level;
        public GameModePlay(Level level)
        {
            this.Level = level;
            this.Level.Parent.Load();
            this.Level.Parent.Play();
        }

        public override void Update()
        {
            this.Level.Update();
        }
        public override void Draw()
        {
            this.Level.Draw();
        }
    }
}