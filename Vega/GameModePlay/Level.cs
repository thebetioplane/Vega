using System;
using System.Collections.Generic;

namespace Vega.GameModePlay
{
    public class Level
    {
        public Track Parent { get; private set; }
        private List<Beat> Beats;

        public Level(Track parent)
        {
            this.Parent = Parent;
            this.Beats = new List<Beat>();
        }
    }
}