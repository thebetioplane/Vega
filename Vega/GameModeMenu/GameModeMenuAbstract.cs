using OpenTK.Graphics.OpenGL;

namespace Vega.GameModeMenu
{
    public abstract class GameModeMenuAbstract : GameMode
    {
        protected Button[] Buttons;
        private int _Selected = -1;
        public int Selected
        {
            get { return this._Selected; }
            set
            {
                Assets.MenuClick.Play();
                if (this._Selected >= 0)
                    this.Buttons[this._Selected].Selected = false;
                this._Selected = value;
                if (this._Selected < 0)
                    this._Selected = this.Buttons.Length - 1;
                else if (this._Selected >= this.Buttons.Length)
                    this._Selected = 0;
                this.Buttons[this._Selected].Selected = true;
            }
        }
        private int Zoom = 0;

        public GameModeMenuAbstract()
            : base()
        {
        }

        public override void OnKeyDown(ActionKey ak)
        {
            switch (ak)
            {
                case ActionKey.MenuDown:
                    this.Selected++;
                    break;
                case ActionKey.MenuUp:
                    this.Selected--;
                    break;
                case ActionKey.MenuEnter:
                case ActionKey.MenuEnterEx:
                    Assets.MenuHit.Play();
                    this.MenuSelect();
                    break;
            }
        }

        public override void OnSwitch()
        {
            this.Zoom = 0;
        }

        public override void Update()
        {
            if (this.Zoom < 20)
                this.Zoom++;
        }

        public override void Draw()
        {
            if (this.Zoom < 20)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, 1600, 900, 0, 0, 4);
                float scale = this.Zoom / 20.0f;
                GL.Scale(scale, scale, 1.0f);
            }
            foreach (Button button in this.Buttons)
                button.Draw();
        }

        protected void MakeButtons(float x, float y, float inc, params string[] names)
        {
            int n = names.Length;
            this.Buttons = new Button[n];
            for (int i = 0; i < n; i++)
                this.Buttons[i] = new Button(names[i], x, y + i * inc);
        }

        protected void SetText(string text)
        {
            if (this.Selected >= 0 && this.Selected < this.Buttons.Length)
                this.Buttons[this.Selected].Text = text;
        }

        protected abstract void MenuSelect();
    }
}
