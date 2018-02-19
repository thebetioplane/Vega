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
        public override void Draw(double t)
        {
            Assets.BlueBeat.Draw(this.Row * 100.0f + 600.0f, (float)(20.0 + 800.0 * t), Vector2.Zero, Color4.White);
        }
        public override void FromBinary(BinaryReaderEx reader)
        {
            base.FromBinary(reader);
            System.Diagnostics.Debug.Assert(this.Row < 4);
        }
    }
}