using System;
using System.IO;

namespace Vega.GameModePlay
{
    public class BinaryWriterEx : BinaryWriter
    {
        public BinaryWriterEx(Stream s) : base(s)
        {
        }
        public void WriteV32(int value)
        {
            this.Write7BitEncodedInt(value);
        }
    }
}