using System;
using System.IO;
using System.Collections.Generic;

namespace Vega.GameModePlay
{
    public class Level
    {
        public Track Parent { get; private set; }
        private List<Beat> Beats;
        public Level(Track parent, string fname)
        {
            this.Parent = parent;
            this.Beats = new List<Beat>();
            for (int i = 0; i < 200; ++i)
            {
                double start = i * this.Parent.Timing[0].SecondsPerBeat + this.Parent.Timing[0].Offset;
                this.Beats.Add(new ShortBeat(start, start, (byte)Main.Self.Rng.Next(4)));
            }
#if sdkfjsdfkjdskf
            this.Parent = parent;
            if (! File.Exists(fname))
                throw new FileNotFoundException("Level not found", fname);
            this.Beats = new List<Beat>();
            using (var fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReaderEx(fs))
            {
                int n = reader.ReadInt32();
                for (int i = 0; i < n; ++i)
                {
                    Beat b = Beat.FromType((BeatType)reader.ReadByte());
                    b.FromBinary(reader);
                    this.Beats.Add(b);
                }
            }
#endif
        }
        private int HeadIndex = 0;
        private int TailIndex = 0;
        private const double ApproachRate = 0.5;
        public void Update()
        {
            double t = this.Parent.GetSeconds();
            while (this.Beats[this.TailIndex].StartTime < t + ApproachRate)
                ++this.TailIndex;
            while (this.Beats[this.HeadIndex].StartTime < t)
                ++this.HeadIndex;
        }
        public void Draw()
        {
            double t = this.Parent.GetSeconds();
            for (int i = this.HeadIndex; i != this.TailIndex; ++i)
            {
                Beat b = this.Beats[i];
                b.Draw(1.0 - (b.StartTime - t) / ApproachRate);
            }
        }
    }
}