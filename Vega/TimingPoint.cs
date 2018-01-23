using System;

namespace Vega
{
    public class TimingPoint : IComparable<TimingPoint>
    {
        private double _BPM;
        public double BPM
        {
            get => this._BPM;
            set
            {
                this._BPM = value;
                this.BeatsPerSecond = value / 60.0;
                this.SecondsPerBeat = 60.0 / value;
            }
        }
        public double SecondsPerBeat { get; private set; }
        public double BeatsPerSecond { get; private set; }
        public double Offset { get; set; }

        public TimingPoint(string csv)
        {
            string[] splt = csv.Split(',');
            if (splt.Length != 2)
                throw new FormatException("Timing point in incorrect format");;
            this.BPM = Convert.ToDouble(splt[0]);
            this.Offset = Convert.ToDouble(splt[1]);
        }
        public TimingPoint(double bpm, long offset)
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