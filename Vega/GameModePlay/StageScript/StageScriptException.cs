using System;

namespace Vega.GameModePlay.StageScript
{
    public class StageScriptException : Exception
    {
        public int Where { get; private set; }

        public StageScriptException(string message, StageScriptOp op, int where)
            : base(string.Format("instr #{0} ({1}): {2}", where, op, message))
        {
            this.Where = where;
        }

        public StageScriptException(string message)
            : base(message)
        {
            this.Where = -1;
        }
    }
}
