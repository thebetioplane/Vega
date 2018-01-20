using System;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class Bullet : Entity
    {
        private Enemy EnemyParent;
        private BulletScriptRunner Runner;
        private float v0 = 0.0f;
        private float v1 = 0.0f;
        private int currentFrame = 0;
        private int targetFrame = 0;
        public float LaunchAngle { get; private set; }
        public float ParentAngle { get; private set; }
        private int Delay;

        public Bullet(GameModePlay parent, Enemy enemyParent, Vector2 position, float launchAngle, int delay, int entryPoint)
            : base(parent, null, position, Color4.White)
        {
            this.EnemyParent = enemyParent;
            this.Runner = new BulletScriptRunner(this.Parent.Level.Script, entryPoint);
            this.Runner.Parent = this;
            this.LaunchAngle = launchAngle;
            this.Angle = launchAngle;
            this.Delay = delay;
            this.ParentAngle = this.Atan2(this.Position, this.Parent.Player.Position);
        }

        public override void Update()
        {
            if (this.Delay > 1)
            {
                this.Position = this.EnemyParent.Position;
                this.Invisible = true;
                this.Delay--;
                if (this.Delay == 1)
                    this.Invisible = false;
                return;
            }
            this.Runner.Tick();
            if (this.targetFrame > 0 && this.currentFrame <= this.targetFrame)
            {
                this.Velocity = this.v0 + (this.v1 - this.v0) * (this.currentFrame / (float)this.targetFrame);
                this.currentFrame++;
            }
            this.Rotation = this.Angle + MathHelper.PiOver2;
            if (this.HitTest(this.Parent.Player.Position, this.Parent.Player.BulletHitbox))
            {
                this.Parent.Player.OnHit();
                this.Delete();
                return;
            }
            base.Update();
        }

        public void SetVelocity(int frameNumber, float v1, float v0)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.targetFrame = frameNumber;
            this.currentFrame = 0;
        }

        public override void Delete()
        {
            this.Parent.BulletsToDelete.Add(this);
        }

        public float GetPlayerAngle()
        {
            return this.Atan2(this.Position, this.Parent.Player.Position);
        }

        public void SetTexture(int which)
        {
            try
            {
                this.Texture = this.Parent.Level.CurrentStage.BulletTexture[which];
            }
            catch (IndexOutOfRangeException)
            {
                this.Runner.ThrowException("Animation " + which + " out of range");
            }
        }
    }
}
