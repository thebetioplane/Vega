namespace Vega
{
    public delegate void TrackBeatHandler(object sender, int downbeat);
    public class TrackEvent
    {
        public Track Track;
        private int Index = -1;
        private double NextBeat = double.PositiveInfinity;
        public event TrackBeatHandler Drum;
        private int MeasureCount = 0;
        public void Poll()
        {
            if (this.Track == null)
                return;
            if (this.Drum == null)
                return;
            double current = this.Track.GetSeconds();
            while (this.Index + 1 != this.Track.Timing.Count
                && current > this.Track.Timing[this.Index + 1].Offset)
            {
                ++this.Index;
                this.MeasureCount = 0;
                var tp = this.Track.Timing[this.Index];
                this.NextBeat =  tp.Offset;
            }
            if (current > this.NextBeat)
            {
                this.NextBeat += this.Track.Timing[this.Index].SecondsPerBeat;
                this.Drum(this, this.MeasureCount);
                this.MeasureCount = (this.MeasureCount == 3) ? 0 : this.MeasureCount + 1;
            }
        }
    }
}