using System.IO;

namespace Vega.GameModePlay.StageScript
{
    public class StageScript
    {
        public const int SSF_VERSION = 0x31465353;
        private StageScriptOp[] Script;
        public int EntryPoint { get; private set; }

        public StageScript(string filename)
        {
            this.ReadFromFile(filename);
        }

        public StageScript(StageScriptOp[] script, int entryPoint)
        {
            this.Script = script;
            this.EntryPoint = entryPoint;
            if (this.Script.Length == 0)
                throw new StageScriptException("Empty stage script");
        }

        public StageScriptOp this[int index]
        {
            get
            {
                if (index >= this.Length)
                    throw new StageScriptException("Execution went past end of script (are you missing an `exit`?)");
                else if (index < 0)
                    throw new StageScriptException("Execution jumps to negative op position");
                return this.Script[index];
            }
        }

        public int Length { get { return this.Script.Length; } }

        public void WriteToFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(SSF_VERSION);
                int n = this.Script.Length;
                writer.Write(this.EntryPoint);
                writer.Write(n);
                for (int i = 0; i < n; i++)
                {
                    OpCode op = this.Script[i].OpCode;
                    writer.Write((byte)op);
                    if (op == OpCode.PushI || op == OpCode.PushF)
                        writer.Write(this.Script[i].Arg.ArgI);
                }
            }
        }

        private void ReadFromFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                if (reader.ReadInt32() != SSF_VERSION)
                    throw new StageScriptException("Incorrect version (or file is not ssf)");
                this.EntryPoint = reader.ReadInt32();
                int n = reader.ReadInt32();
                this.Script = new StageScriptOp[n];
                for (int i = 0; i < n; i++)
                {
                    byte op = reader.ReadByte();
                    if (op == (byte)OpCode.PushI || op == (byte)OpCode.PushF)
                        this.Script[i] = new StageScriptOp(op, reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    else
                        this.Script[i] = new StageScriptOp(op);
                }
            }
            if (this.Script.Length == 0)
                throw new StageScriptException("Empty stage script");
        }

        public void Print(TextWriter writer)
        {
            writer.WriteLine("entry point = {0}", this.EntryPoint);
            for (int i = 0; i < this.Script.Length; i++)
                writer.WriteLine("{0,5} -> {1}", i + 1, this.Script[i]);
        }
    }
}
