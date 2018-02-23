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
#if DEBUGg
            this.Parent = parent;
            this.Beats = new List<Beat>();
            double spb = this.Parent.Timing[0].SecondsPerBeat;
            double off = this.Parent.Timing[0].Offset;
            for (int i = 0; i < 200; ++i)
            {
                double start = i * spb * 1.5 + off;
                this.Beats.Add(new ShortBeat(start, start + spb, (byte)Main.Self.Rng.Next(4)));
            }
            for (int i = 0; i < 100; ++i)
            {
                double start = i * spb * 10 + off;
                this.Beats.Add(new LongBeat(start, start + spb * 4, (byte)Main.Self.Rng.Next(2)));
            }
            this.Beats.Add(new DriftBeat(off, off + 8 * spb, 0, 3));
            this.Beats.Add(new DriftBeat(off + spb * 10, off + 18 * spb, 1, 0));
            this.Beats.Sort();
            using (var fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriterEx(fs))
            {
                writer.Write(this.Beats.Count);
                foreach (var b in this.Beats)
                {
                    writer.Write((byte)b.T);
                    b.ToBinary(writer);
                }
            }
            throw new Exception("File made");
#else
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
        private const double ApproachRate = 1.0;
        public void Reset()
        {
            this.HeadIndex = 0;
            this.TailIndex = 0;
        }
        public void Update()
        {
            double t = this.Parent.GetSeconds();
            while (this.Beats[this.TailIndex].StartTime < t + ApproachRate)
                ++this.TailIndex;
            while (this.Beats[this.HeadIndex].EndTime < t)
                ++this.HeadIndex;
        }
        public void Draw()
        {
            double t = this.Parent.GetSeconds();
            for (int i = this.HeadIndex; i != this.TailIndex; ++i)
            {
                Beat b = this.Beats[i];
                if (b.StartTime == b.EndTime)
                    b.Draw(1.0 - (b.StartTime - t) / ApproachRate);
                else
                    b.Draw(1.0 - (b.StartTime - t) / ApproachRate, 1.0 - (b.EndTime - t) / ApproachRate);
            }
        }
    }
}