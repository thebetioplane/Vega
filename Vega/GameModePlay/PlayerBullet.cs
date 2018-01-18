using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class PlayerBullet : Entity
    {
        public PlayerBullet(GameModePlay parent, float x, float y, float angle)
            : base(parent, Assets.PlayerBullet, new Vector2(x, y), Color4.White)
        {
            this.Angle = angle;
            this.Hitbox = new Circle(this);
            this.Velocity = 20.0f;
        }

        public override void Update()
        {
            base.Update();
            foreach (Enemy enemy in this.Parent.Enemies)
            {
                if (this.HitTest(enemy))
                {
                    enemy.Damage();
                    this.Delete();
                    return;
                }
            }
        }

        public override void Delete()
        {
            this.Parent.PlayerBulletsToDelete.Add(this);
        }
    }
}
