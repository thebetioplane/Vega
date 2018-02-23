using System;
using OpenTK;
using OpenTK.Graphics;

namespace Vega.GameModePlay
{
    public class DriftBeat : Beat
    {
        public byte RowEnd;
        public DriftBeat(double start, double end, byte rowStart, byte rowEnd)
            : base(BeatType.Drift, start, end, rowStart)
        {
            this.RowEnd = rowEnd;
        }
        public override void ToBinary(BinaryWriterEx writer)
        {
            base.ToBinary(writer);
            writer.Write(this.RowEnd);
        }
        public override void FromBinary(BinaryReaderEx reader)
        {
            base.FromBinary(reader);
            this.RowEnd = reader.ReadByte();
            Assert(this.Row < 4, "DriftBeat: Start row too large");
            Assert(this.RowEnd < 4, "DriftBeat: End row too large");
        }
        private static readonly Vector2 BeatOrigin = new Vector2(64.0f, 64.0f);
        public override void Draw(double t0, double t1)
        {
            float y0 = (float)(20.0 + 800.0 * t0);
            float y1 = (float)(20.0 + 800.0 * t1);
            float x0 = this.Row * 200.0f + 200.0f;
            float x1 = this.RowEnd * 200.0f + 200.0f;
            for (int i = 0; i < 200; ++i)
            {
                float f = i / 200.0f;
                Assets.Body.Draw(x1 + (x0 - x1) * f, y1 + (y0 - y1) * f, BeatOrigin, Color4.White);
            }
        }

        public override void Draw(double t0)
        {
        }
    }
}