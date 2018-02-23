using OpenTK;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay
{
    public class ShortBeat : Beat
    {
        public ShortBeat(double start, double end, byte row)
            : base (BeatType.Short, start, end, row)
        {
        }
        private static Vector2 BeatOrigin = new Vector2(25.0f, 25.0f);
        private static Vector2 MidOrigin = new Vector2(25.0f, 0.0f);
        public override void Draw(double t0, double t1)
        {
            float x = this.Row * 100.0f + 600.0f;
            float y0 = (float)(20.0 + 800.0 * t0);
            float y1 = (float)(20.0 + 800.0 * t1);
            Assets.BlueBeat.Draw(x, y0, 50.0f, 50.0f, BeatOrigin, Color4.White);
            Assets.BlueBeat.Draw(x, y1, 50.0f, 50.0f, BeatOrigin, Color4.White);
            Assets.BlueBeatMid.Draw(x, y0, 50.0f, y1 - y0, MidOrigin, Color4.White);
        }
        public override void Draw(double t0)
        {
            Assets.BlueBeat.Draw(this.Row * 100.0f + 600.0f, (float)(20.0 + 800.0 * t0), BeatOrigin, Color4.White);
        }
        public override void FromBinary(BinaryReaderEx reader)
        {
            base.FromBinary(reader);
            
            Assert(this.Row < 4, "ShortBeat: Row is too large");
        }
    }
}