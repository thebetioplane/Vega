using OpenTK;
using Color4 = OpenTK.Graphics.Color4;
using Rectangle = System.Drawing.Rectangle;

namespace Vega.GameModePlay
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
                    this.TextureWidth = 1;
                    this.Height = 1;
                    this.Width = 1;
                    this.Invisible = true;
                }
                else
                {
                    this.TextureWidth = value.Width;
                    this.Height = value.Height;
                    this.Width = this.TextureWidth;
                    this.Invisible = false;
                }
            }
        }
        public Vector2 Position;
        public Color4 Color;
        private int _Width;
        public int Width
        {
            get { return this._Width; }
            set
            {
                if (value >= 0)
                {
                    this._Width = value;
                    this.NumAnimations = this.TextureWidth / this._Width;
                }
                else
                {
                    this.Width = this.TextureWidth;
                    this.NumAnimations = 1;
                }
                this.Origin = new Vector2(this.Width, this.Height) / 2.0f;
            }
        }
        public int NumAnimations { get; private set; }
        public int TextureWidth { get; private set; }
        public int Height { get; private set; }
        private Vector2 Origin;
        public GameModePlay Parent { get; private set; }
        protected float Rotation;
        protected bool Invisible = false;
        protected float Scale = 1.0f;
        private int _Animation = 0;
        protected int Animation
        {
            get { return this._Animation; }
            set
            {
                if (value >= this.NumAnimations)
                    this._Animation = 0;
                else
                    this._Animation = value;
            }
        }

        public Sprite(GameModePlay parent, Texture2D texture, Vector2 position, Color4 color)
        {
            this.NumAnimations = 1;
            this.Parent = parent;
            this.Texture = texture;
            this.Position = position;
            this.Color = color;
            this.Rotation = 0.0f;
        }

        public void Draw()
        {
            if (this.Invisible)
                return;
            this.Texture.Draw(this.Position.X, this.Position.Y, this.Origin, this.Color,
                new Rectangle(this.Width * this.Animation, 0, this.Width, this.Height),
                this.Rotation, this.Scale);
        }
    }
}
