using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega
{
    public enum ActionKey : int { MenuEnter = 0, MenuEnterEx, MenuUp, MenuDown }

    public class Main : GameWindow
    {
        public static Main Self;
        public Random Rng { get; private set; }
        public ConfigFile Config { get; private set; }
        public int TotalFrameCount { get; private set; }
        public string BannerText { get; private set; }
        private GameMode[] LoadedGameMode = new GameMode[Enum.GetNames(typeof(GameModeType)).Length];

        public GameMode CurrentGamemode { get; private set; }

        public Main()
            : base(1024, 576)
        {
            Main.Self = this;
            this.Rng = new Random();
            this.VSync = VSyncMode.Adaptive;
            this.Location = new Point(30, 30);
            this.CurrentKeyboardState = Keyboard.GetState();
            this.BannerText = string.Empty;
            if (! Program.NoNetwork)
            {
                string updaterText = "";
                Maintenance.Updater.DefaultUpdater.TextChanged += (sender, value) =>
                {
                    updaterText = value;
                    this.BannerText = value;
                };
                Maintenance.Updater.DefaultUpdater.PercentChanged += (sender, value) =>
                {
                    this.BannerText = string.Format("{0} ({1}%)", updaterText, value);
                };
                var thread = new System.Threading.Thread(() =>
                {
                    Maintenance.Updater.DefaultUpdater.Run(false);
                });
                thread.Start();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            int x = 0;
            int y = 0;
            int w = this.Height * 16 / 9;
            int h = this.Width * 9 / 16;
            if (w > this.Width)
            {
                w = this.Width;
                h = w * 9 / 16;
                y = (this.Height - h) / 2;
            }
            else
            {
                h = this.Height;
                w = h * 16 / 9;
                x = (this.Width - w) / 2;
            }
            this.Title = string.Format("Window {0}x{1}; Viewport {2}x{3}", this.Width, this.Height, w, h);
            GL.Viewport(x, y, w, h);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Logger.DefaultLogger.WriteLine("OpenGL Version  : {0}", GL.GetString(StringName.Version));
            Logger.DefaultLogger.WriteLine("Shader Version  : {0}", GL.GetString(StringName.ShadingLanguageVersion));
            Logger.DefaultLogger.WriteLine("OpenGL Renderer : {0}", GL.GetString(StringName.Renderer));
            Logger.DefaultLogger.WriteLine("OpenGL Vendor   : {0}", GL.GetString(StringName.Vendor));
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            this.Config = new ConfigFile();
            this.SetKeybinds();
            Assets.LoadContent();
#if DEBUG
            //this.SwitchGamemode(GameModeType.Play);
            this.SwitchGamemode(GameModeType.Menu);
#else
            this.SwitchGamemode(GameModeType.Menu);
#endif
        }

        public void SwitchGamemode(GameModeType type, GameMode obj = null)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 1600, 900, 0, 0, 4);
            byte index = (byte)type;
            if (obj != null)
            {
                Logger.DefaultLogger.WriteLine("Gamemode : {0} ([1}])", type, obj.GetType());
                this.LoadedGameMode[index] = obj;
            }
            else if (this.LoadedGameMode[index] == null)
            {
                Logger.DefaultLogger.WriteLine("Gamemode : {0}", type);
                this.LoadedGameMode[index] = GameMode.FromType(type);
            }
            else
            {
                Logger.DefaultLogger.WriteLine("Gamemode : {0} (cached)", type);
            }
            this.CurrentGamemode = this.LoadedGameMode[index];
            this.CurrentGamemode.OnSwitch();
            GL.ClearColor(this.CurrentGamemode.BackgroundColor);
        }

        private void RefreshGamemodes()
        {
            Logger.DefaultLogger.WriteLine("RefreshGamemodes()");
            for (int i = 0; i < this.LoadedGameMode.Length; i++)
            {
                if (this.LoadedGameMode[i] != null)
                {
                    this.LoadedGameMode[i].Dispose();
                    this.LoadedGameMode[i] = null;
                }
            }
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
            Assets.Dispose();
        }

        public void SetKeybinds()
        {
            this.Keybinds[(int)ActionKey.MenuEnter] = Key.Enter;
            this.Keybinds[(int)ActionKey.MenuEnterEx] = Key.KeypadEnter;
            this.Keybinds[(int)ActionKey.MenuUp] = Key.Up;
            this.Keybinds[(int)ActionKey.MenuDown] = Key.Down;
        }

        private Key[] Keybinds = new Key[Enum.GetNames(typeof(ActionKey)).Length];
        public KeyboardState CurrentKeyboardState { get; private set; }
        private KeyboardState LastKeyboardState;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            this.TotalFrameCount++;
            this.LastKeyboardState = this.CurrentKeyboardState;
            this.CurrentKeyboardState = Keyboard.GetState();
            for (int i = 0; i < this.Keybinds.Length; i++)
            {
                Key key = this.Keybinds[i];
                ActionKey ak = (ActionKey)i;
                if (this.CurrentKeyboardState.IsKeyDown(key) && !this.LastKeyboardState.IsKeyDown(key))
                    this.OnKeyDown(ak);
                else if (this.LastKeyboardState.IsKeyDown(key) && !this.CurrentKeyboardState.IsKeyDown(key))
                    this.OnKeyUp(ak);
            }
            this.CurrentGamemode.Update();
        }

        private void OnKeyDown(ActionKey ak)
        {
            this.CurrentGamemode.OnKeyDown(ak);
        }

        private void OnKeyUp(ActionKey ak)
        {
            this.CurrentGamemode.OnKeyUp(ak);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            this.CurrentGamemode.Draw();
            this.SwapBuffers();
        }

        public void DrawText(string text, Vector2 pos, float scale, Color4 color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                byte c;
                if (text[i] < ' ' || text[i] > '~')
                    c = '.' - 32;
                else
                    c = (byte)(text[i] - 32);
                if (c == 0)
                    continue;
                byte x = (byte)(c % 12);
                byte y = (byte)(c / 12);
                Assets.Font.Draw((pos.X + 18.0f * i) * scale, pos.Y * scale, Vector2.Zero, color, new Rectangle(x * 21, y * 32, 21, 32), 0.0f, scale);
            }
        }

        public void DrawRectangle(RectangleF rect, Color4 color)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Color4(color);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(rect.Left, rect.Top);
            GL.Vertex2(rect.Right, rect.Top);
            GL.Vertex2(rect.Right, rect.Bottom);
            GL.Vertex2(rect.Left, rect.Bottom);
            GL.End();
        }
    }
}
