using System.Collections.Generic;
using System.IO;

namespace Vega
{
    public class Assets
    {
        public static Texture2D Player;
        public static Texture2D PlayerTrail;
        public static Texture2D PlayerBullet;
        public static Texture2D Font;
        public static Texture2D MenuGraphic;
        public static Texture2D Hud;
        public static SoundEffect MenuHit;
        public static SoundEffect MenuClick;
        public static SoundEffect EnemyHit;
        public static SoundEffect EnemyDie;
        public static SoundEffect PlayerHit;
        public static void LoadContent()
        {
            //Player = LocTextureFromFile("player.png");
            //PlayerTrail = LocTextureFromFile("player_trail.png");
            //PlayerBullet = LocTextureFromFile("player_bullet.png");
            
            //Hud = LocTextureFromFile("hud.png");
            using (var imgMap = new AssetMap("VegaImg.dat"))
            {
                MenuGraphic = new Texture2D(imgMap.GetAsset("menu-graphic.png"));
                Font = new Texture2D(imgMap.GetAsset("font.png"));
            }
            MenuHit = new SoundEffect();
            MenuClick = new SoundEffect();
            EnemyHit = new SoundEffect();
            EnemyDie = new SoundEffect();
            PlayerHit = new SoundEffect();
        }

        public static void Dispose()
        {
            // dispose all the assets in LoadContent
        }

        //private static SoundEffect CurrentMusic = null;
        public static void MusicPlayer_Stop()
        {
            //CurrentMusic?.Stop();
        }

        public static void MusicPlayer_Play(int n)
        {
            //CurrentMusic = Songs[n];
            //CurrentMusic.PlayLooping();
        }
    }
}
