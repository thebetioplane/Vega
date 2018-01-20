using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Color4 = OpenTK.Graphics.Color4;
using RectangleF = System.Drawing.RectangleF;

using StageScriptException = Vega.GameModePlay.StageScript.StageScriptException;

namespace Vega.GameModePlay
{
    public class GameModePlay : GameMode
    {
        public Level Level { get; private set; }
        private MainScriptRunner MainRunner;
        public Player Player { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Enemy> EnemiesToDelete { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public List<Bullet> BulletsToDelete { get; private set; }
        public List<PlayerBullet> PlayerBullets { get; private set; }
        public List<PlayerBullet> PlayerBulletsToDelete { get; private set; }
        private PlayerTrail[] PlayerTrails;

        private BackgroundPatternManager BackgroundPatternManager;
        private SlideValue HealthBarLoc;
        private bool _BossFightInProgress = true;
        public bool BossFightInProgress
        {
            get { return this._BossFightInProgress; }
            set
            {
                if (this._BossFightInProgress == value)
                    return;
                this._BossFightInProgress = value;
                if (value)
                {
                    this.HealthBarWidth = 1.0f;
                    this.HealthBarAnimationWidth = 1.0f;
                    this.HealthBarLoc = new SlideValue(-30.0f, 10.0f);
                }
                else
                {
                    this.HealthBarWidth = 0.0f;
                    this.HealthBarLoc = new SlideValue(10.0f, -30.0f);
                    this.MainRunner.BossFightWaiting = false;
                }
            }
        }

        private Vector3 TransitionColor;
        private Vector3 TransitionColorDelta;
        private int TransitionTimer = 0;
        public float HealthBarWidth { get; set; }
        private float HealthBarAnimationWidth = 0.0f;

        private StageScriptException StageScriptError = null;

        public GameModePlay()
            : base()
        {
            this.Enemies = new List<Enemy>();
            this.EnemiesToDelete = new List<Enemy>();
            this.Bullets = new List<Bullet>();
            this.BulletsToDelete = new List<Bullet>();
            this.PlayerBullets = new List<PlayerBullet>();
            this.PlayerBulletsToDelete = new List<PlayerBullet>();
            try
            {
                this.BackgroundPatternManager = new BackgroundPatternManager(this);
                this.Level = new Level(System.IO.Path.Combine(Assets.ASSET_PATH, "lvl"));
                this.MainRunner = new MainScriptRunner(this, this.Level.Script);
                this._BossFightInProgress = true;
                this.BossFightInProgress = false;
            }
            catch (StageScriptException e)
            {
                this.StageScriptError = e;
            }
            this.Player = new Player(this);
            this.PlayerTrails = new PlayerTrail[19];
            for (int i = 0; i < this.PlayerTrails.Length; i++)
                this.PlayerTrails[i] = new PlayerTrail(this.Player, i);
        }

        public override void OnSwitch()
        {
            Main.Self.Music = -1;
        }

        public override void Update()
        {
            try
            {
                if (this.StageScriptError != null)
                    throw this.StageScriptError;
                
                this.MainRunner.Tick();
                if (this.TransitionTimer > 0)
                {
                    this.BackgroundColor = this.ColorFromVec3(this.TransitionColor);
                    this.TransitionColor += this.TransitionColorDelta;
                    this.TransitionTimer--;
                }
                this.Player.Update();
                foreach (PlayerTrail trail in this.PlayerTrails)
                    trail.Update();
                for (int i = 0; i < this.Enemies.Count; i++)
                    this.Enemies[i].Update();
                /**
                foreach (Enemy enemy in this.Enemies)
                    enemy.Update();
                **/
                if (this.EnemiesToDelete.Count > 0)
                {
                    foreach (Enemy enemy in this.EnemiesToDelete)
                        this.Enemies.Remove(enemy);
                    this.EnemiesToDelete.Clear();
                }
                foreach (Bullet bullet in this.Bullets)
                    bullet.Update();
                if (this.BulletsToDelete.Count > 0)
                {
                    foreach (Bullet bullet in this.BulletsToDelete)
                        this.Bullets.Remove(bullet);
                    this.BulletsToDelete.Clear();
                }
                foreach (PlayerBullet bullet in this.PlayerBullets)
                    bullet.Update();
                if (this.PlayerBulletsToDelete.Count > 0)
                {
                    foreach (PlayerBullet bullet in this.PlayerBulletsToDelete)
                        this.PlayerBullets.Remove(bullet);
                    this.PlayerBulletsToDelete.Clear();
                }
                this.HealthBarAnimationWidth += (this.HealthBarWidth - this.HealthBarAnimationWidth) / 20.0f;
                this.HealthBarLoc.Update();
            }
            catch (StageScriptException e)
            {
                if (this.Level != null && this.Level.Script != null)
                {
                    using (FileStream fs = new FileStream("SSF_Error.log", FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine("Level Error");
                        writer.WriteLine(e.Message);
                        writer.WriteLine();
                        writer.WriteLine("SSF File");
                        this.Level.Script.Print(writer);
                    }
                }
                Main.Self.HandleStageScriptError(e);
            }
        }

        public override void Draw()
        {
            this.BackgroundPatternManager.Draw();
            foreach (Enemy enemy in this.Enemies)
                enemy.Draw();
            foreach (PlayerTrail trail in this.PlayerTrails)
                trail.Draw();
            this.Player.Draw();
            foreach (Bullet bullet in this.Bullets)
                bullet.Draw();
            foreach (PlayerBullet bullet in this.PlayerBullets)
                bullet.Draw();
            Assets.Hud.Draw(1000, 0, Vector2.Zero, Color4.White);
            /// TODO
            if (this.HealthBarLoc.Curr > -29.0f)
            {
                RectangleF rect;
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Begin(PrimitiveType.Quads);
                // health bar background
                rect = new RectangleF(10.0f, this.HealthBarLoc.Curr, 980.0f, 20.0f);
                GL.Color4(Color4.Black);
                GL.Vertex2(rect.Left, rect.Top);
                GL.Vertex2(rect.Right, rect.Top);
                GL.Vertex2(rect.Right, rect.Bottom);
                GL.Vertex2(rect.Left, rect.Bottom);
                // healthbar overlay
                rect = new RectangleF(10.0f, this.HealthBarLoc.Curr, 980.0f * this.HealthBarAnimationWidth, 20.0f);
                GL.Color4((byte)64, (byte)131, (byte)255, (byte)0xff);
                GL.Vertex2(rect.Left, rect.Top);
                GL.Vertex2(rect.Right, rect.Top);
                GL.Color4((byte)38, (byte)77, (byte)150, (byte)0xff);
                GL.Vertex2(rect.Right, rect.Bottom);
                GL.Vertex2(rect.Left, rect.Bottom);
                GL.End();
            }
        }

        public void AddEnemy(int entryPoint, float y, float x, bool mirrored, bool boss)
        {
            this.Enemies.Add(new Enemy(this, mirrored, boss, x, y, new EnemyScriptRunner(this.Level.Script, entryPoint)));
        }

        public void AddSlave(int entryPoint, Enemy master, bool mirrored, float angle)
        {
            Slave slave = new Slave(this, master, mirrored, new EnemyScriptRunner(this.Level.Script, entryPoint));
            slave.Angle = angle;
            master.Slaves.Add(slave);
            this.Enemies.Add(slave);
        }

        public override void OnKeyDown(ActionKey ak)
        {
            switch (ak)
            {
                case ActionKey.Up:
                    this.Player.InputUp = true;
                    break;
                case ActionKey.Down:
                    this.Player.InputDown = true;
                    break;
                case ActionKey.Left:
                    this.Player.InputLeft = true;
                    break;
                case ActionKey.Right:
                    this.Player.InputRight = true;
                    break;
                case ActionKey.Primary:
                    this.Player.InputA = true;
                    break;
                /*
            case ActionKey.Seconday:
                this.Player.InputB = true;
                break;
                 */
                case ActionKey.Seconday:
                    this.Player.InputC = true;
                    break;
            }
        }

        public override void OnKeyUp(ActionKey ak)
        {
            switch (ak)
            {
                case ActionKey.Up:
                    this.Player.InputUp = false;
                    break;
                case ActionKey.Down:
                    this.Player.InputDown = false;
                    break;
                case ActionKey.Left:
                    this.Player.InputLeft = false;
                    break;
                case ActionKey.Right:
                    this.Player.InputRight = false;
                    break;
                case ActionKey.Primary:
                    this.Player.InputA = false;
                    break;
                /*
            case ActionKey.Seconday:
                this.Player.InputB = false;
                break;
                 */
                case ActionKey.Seconday:
                    this.Player.InputC = false;
                    break;
            }
        }

        public void SetBackgroundColor(Color4 color)
        {
            this.TransitionTimer = 0;
            this.BackgroundColor = color;
        }

        public void SetBackgroundSpeed(float speed)
        {
            this.BackgroundPatternManager.Speed = speed;
        }

        public void TransitionBackground(int time, Color4 color)
        {
            Vector3 a = this.ColorToVec3(this.BackgroundColor);
            Vector3 b = this.ColorToVec3(color);
            this.TransitionColorDelta = (b - a) / (float)time;
            this.TransitionColor = a;
            this.TransitionTimer = time;
        }

        public void PlaySound(StageScript.StageScriptRunner caller, int which)
        {
            try
            {
                this.Level.CurrentStage.Sounds[which].Play();
            }
            catch (IndexOutOfRangeException)
            {
                caller.ThrowException("Sound effect " + which.ToString() + " out of range");
            }
        }

        public void SetBackground(StageScript.StageScriptRunner caller, int which)
        {
            if (which < 0)
            {
                this.BackgroundPatternManager.Pattern = null;
                return;
            }
            try
            {
                this.BackgroundPatternManager.Pattern = this.Level.CurrentStage.BackgroundTexture[which];
            }
            catch (IndexOutOfRangeException)
            {
                caller.ThrowException("Background pattern " + which.ToString() + " out of range");
            }
        }

        private Vector3 ColorToVec3(Color4 color)
        {
            return new Vector3(color.R, color.G, color.B);
        }

        private Color4 ColorFromVec3(Vector3 vec)
        {
            return new Color4(vec.X, vec.Y, vec.Z, 1.0f);
        }

        public override void Dispose()
        {
            if (this.Level != null)
                this.Level.Dispose();
        }
    }
}
