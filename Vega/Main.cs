using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega
{
    public enum ActionKey : int { MenuEnter = 0, MenuEnterEx, MenuUp, MenuDown, A, B, X, Y }

    public class Main : GameWindow
    {
        public static Main Self;
        public Random Rng { get; private set; }
        public ConfigFile Config { get; private set; }
        public int TotalFrameCount { get; private set; }
        public string BannerText { get; private set; }
        public TrackList TrackList { get; private set; }
        private GameMode[] LoadedGameMode = new GameMode[Enum.GetNames(typeof(GameModeType)).Length];

        public GameMode CurrentGamemode { get; private set; }

        public Main()
            : base(1024, 576)
        {
            Main.Self = this;
            this.Rng = new Random();
            this.VSync = VSyncMode.Adaptive;
            this.Location = new Point(30, 30);
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
            this.TrackList = new TrackList();
            this.Config = new ConfigFile();
            this.SetKeybinds();
            Assets.LoadContent();
#if DEBUG
            this.SwitchGamemode(GameModeType.Play, new GameModePlay.GameModePlay(this.TrackList[0].GetLevel(0)));
#else
            this.SwitchGamemode(GameModeType.Menu);
            if (this.TrackList.Count != 0)
                this.TrackList.GetRandom().Play();
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
                Logger.DefaultLogger.WriteLine("Gamemode : {0} ({1})", type, obj.GetType());
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
            this.Keybinds[(int)ActionKey.A] = Key.A;
            this.Keybinds[(int)ActionKey.B] = Key.B;
            this.Keybinds[(int)ActionKey.X] = Key.X;
            this.Keybinds[(int)ActionKey.Y] = Key.Y;
            Gamepader.Primary.MenuUp += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.MenuUp);
            };
            Gamepader.Primary.MenuDown += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.MenuDown);
            };
            Gamepader.Primary.ButtonA += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.A);
                else
                    this.OnKeyUp(ActionKey.A);
            };
            Gamepader.Primary.ButtonB += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.B);
                else
                    this.OnKeyUp(ActionKey.B);
            };
            Gamepader.Primary.ButtonX += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.X);
                else
                    this.OnKeyUp(ActionKey.X);
            };
            Gamepader.Primary.ButtonY += (sender, sig) =>
            {
                if (sig)
                    this.OnKeyDown(ActionKey.Y);
                else
                    this.OnKeyUp(ActionKey.Y);
            };
        }

        private Key[] Keybinds = new Key[Enum.GetNames(typeof(ActionKey)).Length];
        private bool[] AKDown = new bool[Enum.GetNames(typeof(ActionKey)).Length];
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            this.TotalFrameCount++;
            var keyboardState = Keyboard.GetState();
            for (int i = 0; i < this.Keybinds.Length; i++)
            {
                Key key = this.Keybinds[i];
                ActionKey ak = (ActionKey)i;
                if (keyboardState.IsKeyDown(key) && ! this.AKDown[i])
                {
                    this.OnKeyDown(ak);
                    this.AKDown[i] = true;
                }
                else if (this.AKDown[i] && ! keyboardState.IsKeyDown(key))
                {
                    this.OnKeyUp(ak);
                    this.AKDown[i] = false;
                }
            }
            Gamepader.Primary.Update();
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
