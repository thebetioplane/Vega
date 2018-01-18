using OpenTK;
using Color4 = OpenTK.Graphics.Color4;
using Rectangle = System.Drawing.Rectangle;

namespace Vega.Drawable
{
    public class Sprite
    {
        private Texture2D _Texture;
        public Texture2D Texture
        {
            get { return this._Texture; }
            set
            {
                this._Texture = value;
                if (value == null)
                {
                    this.Width = 1;
                    this.Height = 1;
                    this.Invisible = true;
                    this.Origin = Vector2.Zero;
                }
                else
                {
                    this.Width = value.Width;
                    this.Height = value.Height;
                    this.Invisible = false;
                    this.Origin = new Vector2(this.Width, this.Height) / 2.0f;
                }
            }
        }
        public Vector2 Position;
        public Color4 Color;
        public int Width { get; private set; }
        public int Height { get; private set; }
        protected Vector2 Origin;
        protected float Rotation = 0.0f;
        protected bool Invisible = false;
        protected float Scale = 1.0f;
        public Sprite(Texture2D texture, Vector2 position, Color4 color)
        {
            this.Texture = texture;
            this.Position = position;
            this.Color = color;
        }

        public void Draw()
        {
            if (this.Invisible)
                return;
            this.Texture.Draw(this.Position.X, this.Position.Y, this.Origin, this.Color,
                this.Rotation, this.Scale);
        }
    }
}
