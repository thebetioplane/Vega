using OpenTK;
using OpenTK.Graphics;

namespace Vega.GameModePlay
{
    public class LongBeat : Beat
    {
        public LongBeat(double start, double end, byte row)
            : base (BeatType.Long, start, end, row)
        {
        }
        private static readonly Vector2 HeadOrigin = new Vector2(50.0f, 0.0f);
        private static readonly Vector2 StemOrigin = new Vector2(40.0f, 0.0f);
        public override void Draw(double t0, double t1)
        {
            float x = this.Row * 500.0f + 500.0f;
            float y0 = (float)(20.0 + 800.0 * t0);
            float y1 = (float)(20.0 + 800.0 * t1);
            Assets.RedBeatStem.Draw(x, y0, 80.0f, y1 - y0, StemOrigin, Color4.Red);
        }
        public override void Draw(double t0)
        {
            //Assets.RedBeat.Draw(this.Row * 500.0f + 500.0f, (float)(20.0 + 800.0 * t0), BeatOrigin, Color4.White);
        }
        public override void FromBinary(BinaryReaderEx reader)
        {
            base.FromBinary(reader);
            Assert(this.Row < 2, "LongBeat: Row is too large");
        }
    }
}