﻿using System;

namespace Vega.GameModeMenu
{
    public class GameModeSelect : GameModeMenuAbstract
    {
        public GameModeSelect()
            : base()
        {
            this.MakeButtons(25.0f, 5.0f, 25.0f, "Play from beginning", "Stage select", "Back");
        }

        protected override void MenuSelect()
        {
            switch (this.Selected)
            {
                case 0:
                    Main.Self.SwitchGamemode(GameModeType.Play);
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
