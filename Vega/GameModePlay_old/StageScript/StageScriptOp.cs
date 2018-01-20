using System;

namespace Vega.GameModePlay.StageScript
{
    public class StageScriptOp
    {
        public OpCode OpCode { get; private set; }
        public ScriptStackEntry Arg { get; private set; }

        public StageScriptOp(OpCode opCode, params byte[] args)
        {
            this.OpCode = opCode;
            bool isFloat = this.OpCode == OpCode.PushF;
            if (args.Length == 4)
                this.Arg = new ScriptStackEntry(BitConverter.ToInt32(args, 0), BitConverter.ToSingle(args, 0), isFloat);
            else
                this.Arg = new ScriptStackEntry(0, 0.0f, isFloat);
        }

        public StageScriptOp(float value)
            : this(OpCode.PushF, BitConverter.GetBytes(value)) { }

        public StageScriptOp(int value)
            : this(OpCode.PushI, BitConverter.GetBytes(value)) { }

        public StageScriptOp(byte opCode, params byte[] args)
            : this((OpCode)opCode, args) { }

        public override string ToString()
        {
            if (this.OpCode == OpCode.PushI
                || this.OpCode == OpCode.PushF)
                return string.Format("{0} ({1})", this.OpCode, this.Arg);
            else
                return this.OpCode.ToString();
        }
    }
}
