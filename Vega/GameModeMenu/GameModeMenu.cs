using System;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModeMenu
{
    public class GameModeMenu : GameModeMenuAbstract
    {
        private int T = 0;
        private float C = 0;
        public float Bpm = 60.0f;

        public GameModeMenu()
            : base()
        {
            this.MakeButtons(25.0f, 200.0f, 25.0f, "Play", "Options", "Exit");
        }

        public override void OnSwitch()
        {
            base.OnSwitch();
        }

        public void UseTiming(TimingPoint tp)
        {
            this.Bpm = tp.BPM;
            this.T = (int)(tp.Offset / 60);
        }

        protected override void MenuSelect()
        {
            switch (this.Selected)
            {
                case 0:
                    Main.Self.SwitchGamemode(GameModeType.Select);
                    break;
                case 1:
                    Main.Self.SwitchGamemode(GameModeType.Option);
                    break;
                case 2:
                    Main.Self.Exit();
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            this.C = (float)Math.Cos(MathHelper.Pi * this.Bpm * this.T / 3600.0f) / 3.0f + 0.6666f;
            ++this.T;
        }

        public override void Draw()
        {
            base.Draw();
            Main.Self.DrawText("Vega", new Vector2(4.0f, 4.0f), 15.0f, new Color4(this.C, 0.0f, 0.0f, 1.0f));
            Main.Self.DrawText(Main.Self.BannerText, new Vector2(35.0f, 420.0f), 2.0f, Color4.YellowGreen);
        }
    }
}
