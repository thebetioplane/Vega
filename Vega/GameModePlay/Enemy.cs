using System;
using System.Collections.Generic;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public delegate Vector2 Interpolater(float t);

    public class Enemy : Entity
    {
        private EnemyScriptRunner Runner;
        public bool Mirrored { get; private set; }
        protected bool IsBoss;
        protected bool IsSlave;
        public int Delay = 0;
        public int Health;
        private int _MaxHealth;
        public int MaxHealth
        {
            get { return this._MaxHealth; }
            set
            {
                this._MaxHealth = value;
                this.Health = value;
            }
        }
        public List<Slave> Slaves = new List<Slave>(4);

        private int MaxMovingFrames = 0;
        private int MovingFrames = 0;
        private Interpolater MovingDelta = (t) => Vector2.Zero;

        public Enemy(GameModePlay parent, bool mirrored, bool boss, float x, float y, EnemyScriptRunner runner)
            : base(parent, null, new Vector2(x, y), Color4.White)
        {
            this.Hitbox = new Circle(this);
            this.Runner = runner;
            this.Runner.Parent = this;
            this.Mirrored = mirrored;
            this.IsBoss = boss;
            this.IsSlave = false;
            this.MaxHealth = 6;
            if (this.Mirrored)
                this.Position.X = 900.0f - this.Position.X;
        }

        public override void Update()
        {
            this.Runner.Tick();
            if (this.MovingFrames > 0)
            {
                this.Position += this.MovingDelta(this.MovingFrames / (float)this.MaxMovingFrames);
                this.MovingFrames--;
            }
            if (this.IsBoss)
                this.Parent.HealthBarWidth = this.Health / (float)this.MaxHealth;
            base.Update();
        }

        public override void Delete()
        {
            if (this.IsBoss)
                this.Parent.BossFightInProgress = false;
            foreach (Slave slave in this.Slaves)
                slave.Delete();
            this.Parent.EnemiesToDelete.Add(this);
        }

        public void CreateBullet(int fillMode, int entryPoint, int max, int num, float launchAngle)
        {
            float diff = MathHelper.TwoPi / (float)max;
            float angle;
            if (fillMode == 1)
                angle = launchAngle - diff * (num - 1);
            else if (fillMode == 2)
                angle = launchAngle - diff * (num - 1) / 2.0f;
            else
                angle = launchAngle;
            for (int i = 0; i < num; i++)
            {
                this.Parent.Bullets.Add(new Bullet(this.Parent, this, this.Position, angle, this.Delay * i, entryPoint));
                angle += diff;
            }
        }

        public void Damage()
        {
            if (this.IsSlave)
                return;

            if (this.Health > 0)
            {
                Assets.EnemyHit.Play();
                this.Health--;
            }
            else
            {
                Assets.EnemyDie.Play();
                this.Delete();
            }
        }

        public void SetTexture(int which)
        {
            try
            {
                this.Texture = this.Parent.Level.CurrentStage.EnemyTexture[which];
                this.Width = this.Parent.Level.CurrentStage.EnemyTextureWidth[which];
                this.NumFrames = this.Parent.Level.CurrentStage.EnemyTextureFrame[which];
            }
            catch (IndexOutOfRangeException)
            {
                this.Runner.ThrowException("Animation " + which + " out of range");
            }
        }

        public void MoveLinear(int time, float y, float x)
        {
            this.MovingFrames = time;
            this.MaxMovingFrames = time;
            Vector2 height = (new Vector2(x, y) - this.Position) / (float)time;
            this.MovingDelta = (t) => height;
        }

        public void MoveCubic(int time, float y, float x)
        {
            this.MovingFrames = time;
            this.MaxMovingFrames = time;
            Vector2 height = (new Vector2(x, y) - this.Position) / (float)time * 4.0f;
            this.MovingDelta = (t) =>
            {
                if (t >= 0.5)
                    return height * (1.0f - t);
                else
                    return height * t;
            };
        }

        public float GetPlayerAngle()
        {
            return this.Atan2(this.Position, this.Parent.Player.Position);
        }
    }
}
