namespace Vega.GameModePlay
{
    public class LongBeat : Beat
    {
        public LongBeat(double start, double end, byte row)
            : base (BeatType.Long, start, end, row)
        {
        }
        public override void Draw(double t)
        {

        }
    }
}