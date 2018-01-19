using System;
using OpenTK;
using OpenTK.Input;

namespace Vega
{
    public delegate void GamepadSignal(object sender, bool sig);
    public class Gamepader
    {
        public static Gamepader Primary = new Gamepader(0);
        private int Index;
        private Gamepader(int index)
        {
            this.Index = index;
        }

        public event GamepadSignal MenuUp;
        public event GamepadSignal MenuDown;
        public event GamepadSignal ButtonA;
        public event GamepadSignal ButtonB;
        public event GamepadSignal ButtonX;
        public event GamepadSignal ButtonY;
        private bool[] KeyState = new bool[8];
        public void Update()
        {
            var state = GamePad.GetState(this.Index);
            var stick = state.ThumbSticks;
            var btn = state.Buttons;
            if (this.MenuUp != null)
            {
                this.ButtonTest(stick.Left.Y > 0.8f, 0, this.MenuUp);
                this.ButtonTest(stick.Right.Y > 0.8f, 1, this.MenuUp);
            }
            if (this.MenuDown != null)
            {
                this.ButtonTest(stick.Left.Y < -0.8f, 2, this.MenuDown);
                this.ButtonTest(stick.Right.Y < -0.8f, 3, this.MenuDown);
            }
            if (this.ButtonA != null)
                this.ButtonTest(state.Buttons.A == ButtonState.Pressed, 4, this.ButtonA);
            if (this.ButtonA != null)
                this.ButtonTest(state.Buttons.B == ButtonState.Pressed, 5, this.ButtonB);
            if (this.ButtonA != null)
                this.ButtonTest(state.Buttons.X == ButtonState.Pressed, 6, this.ButtonX);
            if (this.ButtonA != null)
                this.ButtonTest(state.Buttons.Y == ButtonState.Pressed, 7, this.ButtonY);
        }

        private void ButtonTest(bool test, int index, GamepadSignal sig)
        {
            if (test && ! this.KeyState[index])
            {
                sig(this, true);
                this.KeyState[index] = true;
            }
            else if (this.KeyState[index] && ! test)
            {
                sig(this, false);
                this.KeyState[index] = false;
            }
        }
    }
}