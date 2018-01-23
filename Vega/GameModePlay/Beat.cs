using System;

namespace Vega.GameModePlay
{
    public enum BeatType : byte
    {

    }
    public class Beat : IComparable<Beat>
    {
        public double StartTime;
        public BeatType T;

        public Beat()
        {

        }
        public int CompareTo(Beat other)
        {
            return this.StartTime.CompareTo(other.StartTime);
        }
    }
}