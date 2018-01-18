namespace Vega.GameModeMenu
{
    public class GameModeOption : GameModeMenuAbstract
    {
        public GameModeOption()
            : base()
        {
            this.MakeButtons(5.0f, 5.0f, 15.0f, "Back", "--------");
        }

        private bool Modified;

        public override void OnSwitch()
        {
            base.OnSwitch();
            this.Modified = false;
        }

        protected override void MenuSelect()
        {
            switch (this.Selected)
            {
                case 0:
                    if (this.Modified)
                    {
                        Main.Self.Config.Save();
                        this.Modified = false;
                    }
                    Main.Self.SwitchGamemode(GameModeType.Menu);
                    break;
                case 1:
                    this.SetText("~~~~~~~~");
                    break;
            }
            this.Modified = true;
        }
    }
}
