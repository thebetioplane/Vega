using System;

namespace Vega.GameModeMenu
{
    public class GameModeSelect : GameModeMenuAbstract
    {
        public GameModeSelect()
            : base()
        {
            this.MakeButtons(25.0f, 5.0f, 25.0f, "Play", "Play", "Back");
        }

        protected override void MenuSelect()
        {
            switch (this.Selected)
            {
                case 0:
                    if (Main.Self.TrackList.Count == 0)
                    {
                        this.SetText("You have no songs");
                    }
                    else
                    {
                        Main.Self.SwitchGamemode(GameModeType.Play,
                            new GameModePlay.GameModePlay(Main.Self.TrackList[0].GetLevel(0)));
                    }
                    break;
                case 1:
                    this.SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    break;
                case 2:
                    Main.Self.SwitchGamemode(GameModeType.Menu);
                    break;
            }
        }
    }
}
