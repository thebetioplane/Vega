using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModeMenu
{
    public class Button
    {
        public string Text { get;set; }
        public bool Selected { get; set; }

        private Vector2 Position;
        private int When = 0;
        private Color4 Color
        {
            get
            {
                if (this.Selected)
                {
                    this.When++;
                    this.When %= 10;
                    if (this.When > 5)
                        return Color4.Red;
                    else
                        return Color4.Yellow;
                }
                else
                {
                    return Color4.White;
                }
            }
        }

        public Button(string text, float x, float y)
        {
            this.Selected = false;
            this.Text = text;
            this.Position = new Vector2(x, y);
        }

        public void Draw()
        {
            Main.Self.DrawText(this.Text, this.Position, 3.0f, this.Color);
        }
    }
}
