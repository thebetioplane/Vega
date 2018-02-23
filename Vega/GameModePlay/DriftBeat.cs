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
        public override void Draw(double t0, double t1)
        {

        }

        public override void Draw(double t0)
        {
        }
    }
}