using System;
using System.IO;
using System.Collections.Generic;

namespace Vega
{
    public class TrackList : List<Track>
    {
        public TrackList()
        {
            if (! Directory.Exists("Songs"))
                Directory.CreateDirectory("Songs");
            foreach (var dir in Directory.EnumerateDirectories("Songs"))
            {
                try
                {
                    this.Add(new Track(dir));
                }
                catch
                {
                }
            }
        }

        public Track GetRandom()
        {
            return this[Main.Self.Rng.Next(this.Count)];
        }
    }
}