using System;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public abstract class Entity : Sprite
    {
        public float Velocity;
        protected Circle Hitbox;
        private float _Angle = -1.0f;
        public float Angle
        {
            get { return this._Angle; }
            set
            {
                if (value == this._Angle)
                    return;
                this._Angle = value;
                this._Sin = (float)Math.Sin(value);
                this._Cos = (float)Math.Cos(value);
            }
        }
        private float _Cos = 0.0f;
        protected float Cos { get { return this._Cos; } }
        private float _Sin = 0.0f;
        protected float Sin { get { return this._Sin; } }
        public float AngleSpeed { get; set; }
        protected int NumFrames = 0;
        private int Frame = 0;

        public Entity(GameModePlay parent, Texture2D texture, Vector2 position, Color4 color)
            : base(parent, texture, position, color)
        {
            this.Hitbox = new Circle();
            this.Angle = 0.0f;
        }

        public virtual void Update()
        {
            this.Frame++;
            if (this.Frame >= this.NumFrames)
            {
                this.Animation++;
                this.Frame = 0;
            }
            this.Angle += this.AngleSpeed;
            this.Position.X += this.Cos * this.Velocity;
            this.Position.Y += this.Sin * this.Velocity;
            if (this.Position.X < -500 || this.Position.X > 1500
                || this.Position.Y < -500 || this.Position.Y > 1500)
            {
                this.Delete();
            }
        }

        protected float Atan2(Vector2 a, Vector2 b)
        {
            return (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        protected bool HitTest(Entity other)
        {
            float a = this.Position.X + this.Hitbox.Center.X - other.Position.X - other.Hitbox.Center.X;
            float b = this.Position.Y + this.Hitbox.Center.Y - other.Position.Y - other.Hitbox.Center.Y;
            float c = this.Hitbox.Radius + other.Hitbox.Radius;
            return a * a + b * b <= c * c;
        }

        protected bool HitTest(Vector2 position, Circle hitbox)
        {
            float a = this.Position.X + this.Hitbox.Center.X - position.X - hitbox.Center.X;
            float b = this.Position.Y + this.Hitbox.Center.Y - position.Y - hitbox.Center.Y;
            float c = this.Hitbox.Radius + hitbox.Radius;
            return a * a + b * b <= c * c;
        }

        public abstract void Delete();


        public void DefaultHitbox()
        {
            this.Hitbox = new Circle(this);
        }

        public void SetHitbox(float r, float y, float x)
        {
            this.Hitbox = new Circle(x, y, r);
        }
    }
}
