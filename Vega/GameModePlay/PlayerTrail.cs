using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class PlayerTrail : Sprite
    {
        private const float STARTING_ALPHA = 0.5f;

        private Player Player;
        private int LifeSpan;

        public PlayerTrail(Player player, int index)
            : base(player.Parent, Assets.PlayerTrail, player.Position, new Color4(1.0f, 1.0f, 1.0f, STARTING_ALPHA))
        {
            this.Player = player;
            this.LifeSpan = index * 4;
        }

        public void Update()
        {
            this.Rotation += 0.10f;
            this.Scale += 0.05f;
            this.Position.Y += 3.0f;
            this.Color.A -= 0.01f;
            this.LifeSpan--;
            if (this.LifeSpan <= 0)
            {
                this.Position = this.Player.Position;
                this.LifeSpan = 60;
                this.Scale = 1.0f;
                this.Rotation = 0.0f;
                this.Color.A = STARTING_ALPHA;
            }
        }
    }
}
