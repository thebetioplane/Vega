using System.Collections.Generic;
using System.IO;

namespace Vega
{
    public class Assets
    {
        public static Texture2D Font;
        public static Texture2D BlueBeat;
        public static void LoadContent()
        {
            using (var imgMap = new AssetMap("VegaImg.dat"))
            {
                Font = new Texture2D(imgMap.GetAsset("font.png"));
                BlueBeat = new Texture2D(imgMap.GetAsset("blue_beat.png"));
            }
        }

        public static void Dispose()
        {
            // dispose all the assets in LoadContent
        }
    }
}
