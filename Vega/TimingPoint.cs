using System;

namespace Vega
{
    public class TimingPoint : IComparable<TimingPoint>
    {
        public float BPM;
        public long Offset;

        public TimingPoint(string csv)
        {
            string[] splt = csv.Split(',');
            if (splt.Length != 2)
                throw new FormatException("Timing point in incorrect format");;
            this.BPM = Convert.ToSingle(splt[0]);
            this.Offset = Convert.ToInt64(splt[1]);
        }
        public TimingPoint(float bpm, long offset)
        {
            this.BPM = bpm;
            this.Offset = offset;
        }
        public int CompareTo(TimingPoint other)
        {
            return this.Offset.CompareTo(other.Offset);
        }
    }
}