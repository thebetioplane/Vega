using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class BackgroundPatternManager
    {
        private Texture2D _Pattern = null;
        public Texture2D Pattern
        {
            get { return this._Pattern; }
            set
            {
                this._Pattern = value;
                if (value != null)
                    this.PerY = 1000 / value.Height + 1;
            }
        }
        private int PerY;
        private float Scroll;
        public float Speed { get; set; }
        private GameModePlay Parent;
        public BackgroundPatternManager(GameModePlay parent)
        {
            this.Parent = parent;
            this.Pattern = null;
            this.Scroll = 0.0f;
            this.Speed = 2.0f;
        }

        public void Draw()
        {
            if (this.Pattern == null)
                return;            
            for (int i = -1; i < this.PerY; i++)
                this.Pattern.Draw(0, this.Pattern.Height * i + this.Scroll, Vector2.Zero, this.Parent.BackgroundColor);
            this.Scroll += this.Speed;
            while (this.Scroll > this.Pattern.Height)
                this.Scroll -= this.Pattern.Height;
        }
    }
}
