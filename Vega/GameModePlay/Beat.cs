using System;

namespace Vega.GameModePlay
{
    public enum BeatType : byte { Short, Long, Drift }
    public abstract class Beat : IComparable<Beat>
    {
        public double StartTime;
        public double EndTime;
        public BeatType T;
        public byte Row;
        protected Beat(BeatType t, double start, double end, byte row)
        {
            this.T = t;
            this.StartTime = start;
            this.EndTime = end;
            this.Row = row;
        }
        public static Beat FromType(BeatType t)
        {
            switch (t)
            {
                case BeatType.Short:
                    return new ShortBeat(0, 0, 0);
                case BeatType.Long:
                    return new LongBeat(0, 0, 0);
                case BeatType.Drift:
                    return new DriftBeat(0, 0, 0, 0);
                default:
                    throw new ArgumentException(string.Format("BeatType `{0}` no recognized", t));
            }
        }
        public int CompareTo(Beat other)
        {
            return this.StartTime.CompareTo(other.StartTime);
        }
        public static bool operator < (Beat a, Beat b)
        {
            return a.StartTime < b.StartTime;
        }
        public static bool operator > (Beat a, Beat b)
        {
            return a.StartTime > b.StartTime;
        }
        public virtual void ToBinary(BinaryWriterEx writer)
        {
            writer.Write(this.StartTime);
            writer.Write(this.EndTime);
            writer.Write(this.Row);
        }
        public virtual void FromBinary(BinaryReaderEx reader)
        {
            this.StartTime = reader.ReadDouble();
            this.EndTime = reader.ReadDouble();
            this.Row = reader.ReadByte();
            Assert(this.StartTime >= 0, "StartTime less than zero");
            Assert(this.EndTime >= this.StartTime, "EndTime before StartTime");
            // assertions for rows will be done by overriding
            // in base classes
        }
        // For when given a start and end
        public abstract void Draw(double t0, double t1);
        // For when start and end are the same
        public abstract void Draw(double t0);
        protected static void Assert(bool test, string why)
        {
            if (! test)
                throw new Exception("Beat: Assert failed: " + why);
        }
    }
}