using System;
using OpenTK.Graphics.OpenGL;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega
{
    public enum GameModeType : byte
    {
        Menu, Option, Select, Play
    }

    public class GameMode : IDisposable
    {
        public GameModeType Type { get; private set; }

        public static GameMode FromType(GameModeType type)
        {
            GameMode gameMode = GetGamemode(type);
            gameMode.Type = type;
            return gameMode;
        }

        private Color4 _BackgroundColor = Color4.Black;
        public Color4 BackgroundColor
        {
            get { return this._BackgroundColor; }
            set
            {
                this._BackgroundColor = value;
                GL.ClearColor(value);
            }
        }

        private static GameMode GetGamemode(GameModeType type)
        {
            switch (type)
            {
                case GameModeType.Menu:
                    return new GameModeMenu.GameModeMenu();
                case GameModeType.Option:
                    return new GameModeMenu.GameModeOption();
                case GameModeType.Select:
                    return new GameModeMenu.GameModeSelect();
                case GameModeType.Play:
                    throw new ArgumentException("NO SUCH GAMEMODE");
                default:
                    throw new ArgumentException("No such gamemode");
            }
        }

        public virtual void OnSwitch() { }

        public virtual void Update() { }

        public virtual void Draw() { }

        public virtual void OnKeyDown(ActionKey ak) { }

        public virtual void OnKeyUp(ActionKey ak) { }

        public virtual void Dispose() { }
    }
}
