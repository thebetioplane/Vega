using System.Collections.Generic;
using System.IO;

namespace Vega
{
    public class Assets
    {
        public static Texture2D Pixel;
        public static Texture2D Font;
        public static Texture2D BlueBeat;
        public static Texture2D BlueBeatMid;
        public static Texture2D RedBeatStem;
        public static Texture2D Body;
        public static void LoadContent()
        {
            using (var imgMap = new AssetMap("VegaImg.dat"))
            {
                Pixel = new Texture2D(imgMap.GetAsset("pixel.png"));
                Font = new Texture2D(imgMap.GetAsset("font.png"));
                BlueBeat = new Texture2D(imgMap.GetAsset("blue_beat.png"));
                BlueBeatMid = new Texture2D(imgMap.GetAsset("blue_beat_mid.png"));
                RedBeatStem = new Texture2D(imgMap.GetAsset("red_beat_stem.png"));
                Body = new Texture2D(imgMap.GetAsset("body.png"));
            }
        }

        public static void Dispose()
        {
            // dispose all the assets in LoadContent
        }
    }
}
