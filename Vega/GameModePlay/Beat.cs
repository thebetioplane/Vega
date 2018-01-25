using System;

namespace Vega.GameModePlay
{
    public enum BeatType : byte
    {
        White0, White1, White2, White3,
        Red0, Red1,
        Blue,
    }
    public class Beat : IComparable<Beat>
    {
        public double StartTime;
        public double EndTime;
        public BeatType T;
        public Beat(BeatType t, double start, double end)
        {
            this.T = t;
            this.StartTime = start;
            this.EndTime = end;
        }
        public int CompareTo(Beat other)
        {
            return this.StartTime.CompareTo(other.StartTime);
        }
    }
}