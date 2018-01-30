using System;
using System.IO;

namespace Vega.GameModePlay
{
    public class BinaryReaderEx : BinaryReader
    {
        public BinaryReaderEx(Stream s)
            : base(s)
        {
        }
        public int ReadV32()
        {
            return this.Read7BitEncodedInt();
        }
    }
}