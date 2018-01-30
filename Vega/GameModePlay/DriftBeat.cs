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
        }
        public override void Draw(double t)
        {

        }
    }
}