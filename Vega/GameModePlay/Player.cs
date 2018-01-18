using System;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class Player : Sprite
    {
        private const float SQRT_TWO = 1.4142135f;

        public bool InputUp { get; set; }
        public bool InputDown { get; set; }
        public bool InputLeft { get; set; }
        public bool InputRight { get; set; }
        public bool InputA { get; set; }
        public bool InputB { get; set; }
        public bool InputC { get; set; }
        private readonly float Speed;
        private readonly float SlowSpeed;
        private int ShootClock = 0;
        public Circle BulletHitbox { get; private set; }
        public Circle PickupHitbox { get; private set; }

        public Player(GameModePlay parent)
            : base(parent, Assets.Player, new Vector2(450, 450), Color4.White)
        {
            this.InputUp = false;
            this.InputDown = false;
            this.InputLeft = false;
            this.InputRight = false;
            this.InputA = false;
            this.InputB = false;
            this.InputC = false;
            this.Speed = 10.0f;
            this.SlowSpeed = 5.0f;
            this.BulletHitbox = new Circle(0.0f, 0.0f, 3.0f);
            this.PickupHitbox = new Circle(0.0f, 0.0f, 40.0f);
        }

        public void Update()
        {
            float dx = 0.0f;
            float dy = 0.0f;
            float ds = this.InputC ? this.SlowSpeed : this.Speed;
            if (this.InputUp)
                dy = -ds;
            else if (this.InputDown)
                dy = ds;
            if (this.InputLeft)
                dx = -ds;
            else if (this.InputRight)
                dx = ds;
            bool inputX = dx != 0;
            bool inputY = dy != 0;
            if (inputX && inputY)
            {
                dx /= SQRT_TWO;
                dy /= SQRT_TWO;
            }
            if (inputX)
            {
                this.Position.X += dx;
                this.Position.X = MathHelper.CVegap(this.Position.X, this.Width / 2.0f, 1000 - this.Width / 2.0f);
            }
            if (inputY)
            {
                this.Position.Y += dy;
                this.Position.Y = MathHelper.CVegap(this.Position.Y, this.Height / 2.0f, 900 - this.Height / 2.0f);
            }
            if (this.ShootClock > 0)
                this.ShootClock--;
            else if (this.InputA)
                this.Fire();
            this.Rotation += (Math.Abs(dx) + Math.Abs(dy)) / 75.0f + 0.05f;
        }
                       
        public void Fire()
        {
            this.Parent.PlayerBullets.Add(new PlayerBullet(this.Parent, this.Position.X - 
                this.Width / 2.0f, this.Position.Y, 4.7123889f));
            this.Parent.PlayerBullets.Add(new PlayerBullet(this.Parent, this.Position.X + 
                this.Width / 2.0f, this.Position.Y, 4.7123889f));
            this.ShootClock = 5;
        }

        public void OnHit()
        {
            Assets.PlayerHit.Play();
        }
    }
}
