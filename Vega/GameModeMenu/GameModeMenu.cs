using System;
using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModeMenu
{
    public class GameModeMenu : GameModeMenuAbstract
    {
        private int T = 0;
        private float C = 0.2f;
        private TrackEvent TrackEvent;
        public GameModeMenu()
            : base()
        {
            this.MakeButtons(25.0f, 200.0f, 25.0f, "Play", "Options", "Exit");
            this.TrackEvent = new TrackEvent();
            this.TrackEvent.Drum += (sender, downbeat) =>
            {
                this.C = 1.0f;
            };
        }

        public override void OnSwitch()
        {
            base.OnSwitch();
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
            if (this.TrackEvent.Track == null || this.TrackEvent.Track != Track.GetSPCurrentTrack())
            {
                this.TrackEvent.Track = Track.GetSPCurrentTrack();
            }
            this.TrackEvent.Poll();
            base.Update();
            if (this.C > 0.2)
                this.C -= 0.05f;
            //this.C = (float)Math.Cos(MathHelper.Pi * this.Bpm * this.T / 3600.0f) / 3.0f + 0.6666f;
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
