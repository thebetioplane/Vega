using System;
using Color4 = OpenTK.Graphics.Color4;

namespace Vega.GameModePlay.StageScript
{
    public abstract class StageScriptRunner
    {
        private static readonly Random Rng = new Random();
        private StageScript Script;
        protected ScriptStack Stack;
        protected bool Suspended = false;
        private int Ptr;
        private int Wait;
        private bool Exited;
        private int LoopEntry;
        private int LoopCount;

        public StageScriptRunner(StageScript script, int entryPoint, int stackSize)
        {
            this.Script = script;
            this.Ptr = entryPoint;
            this.Stack = new ScriptStack(this, stackSize);
            this.Wait = 0;
       }

        public StageScriptRunner(StageScript script)
            : this(script, script.EntryPoint, 20)
        {
        }

        public void Tick()
        {
            if (this.Exited)
                return;
            if (this.Wait > 0)
            {
                this.Wait--;
                return;
            }
            while (!this.Suspended && this.Wait == 0)
            {
                this.Exited = this.Script[this.Ptr].OpCode == OpCode.Exit;
                if (this.Exited)
                    break;
                this.TopLevelDoOp(this.Script[this.Ptr]);
                this.Ptr++;
            }
        }

        private void TopLevelDoOp(StageScriptOp op)
        {
            int jmpTo;
            switch (op.OpCode)
            {
                case OpCode.NoOp:
                    Logger.DefaultLogger.WriteLine("NoOp called at {0}", this.Ptr);
                    break;
                case OpCode.Debug:
                    jmpTo = this.Stack.Pop().ArgI;
                    switch (jmpTo)
                    {
                        case 0:
                            this.ThrowException("Debug breakpoint");
                            break;
                        case 1:
                            this.ThrowException("Stack Dump\n" + this.Stack.ToString());
                            break;
                        default:
                            this.ThrowException("Debug called with parameter = " + jmpTo.ToString());
                            break;
                    }
                    break;
                case OpCode.PushI:
                case OpCode.PushF:
                    this.Stack.Push(op.Arg);
                    break;
                case OpCode.Dup:
                    this.Stack.Push(this.Stack.Top);
                    break;
                case OpCode.Pop:
                    this.Stack.Pop();
                    break;
                case OpCode.Jmp:
                    this.Ptr = this.Stack.Pop().ArgI - 1;
                    break;
                case OpCode.Jz:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() == 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Jnz:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() != 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Jg:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() > 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Jge:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() >= 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Jl:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() < 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Jle:
                    jmpTo = this.Stack.Pop().ArgI - 1;
                    if (this.Stack.Cmp() <= 0)
                        this.Ptr = jmpTo;
                    break;
                case OpCode.Inc:
                    this.Stack.Inc();
                    break;
                case OpCode.Dec:
                    this.Stack.Dec();
                    break;
                case OpCode.Neg:
                    this.Stack.Neg();
                    break;
                case OpCode.Add:
                    this.Stack.Add();
                    break;
                case OpCode.Sub:
                    this.Stack.Sub();
                    break;
                case OpCode.Mult:
                    this.Stack.Mult();
                    break;
                case OpCode.Div:
                    this.Stack.Div();
                    break;
                case OpCode.LoopSetup:
                    this.LoopEntry = this.Stack.Pop().ArgI;
                    this.LoopCount = this.Stack.Pop().ArgI;
                    break;
                case OpCode.Loop:
                    if (--this.LoopCount > 0)
                        this.Ptr = this.LoopEntry - 1;
                    break;
                case OpCode.RandomI:
                    this.Stack.Push(new ScriptStackEntry(this.RandI(), 0.0f, false));
                    break;
                case OpCode.RandomF:
                    this.Stack.Push(new ScriptStackEntry(0, this.RandF(), true));
                    break;
                case OpCode.Wait:
                    this.Wait = this.Stack.Pop().ArgI;
                    break;
                default:
                    this.DoOp(op);
                    break;
            }
        }

        protected abstract void DoOp(StageScriptOp op);

        private int RandI()
        {
            int b = this.Stack.Pop().ArgI;
            int a = this.Stack.Pop().ArgI;
            if (a > b)
                this.ThrowException("Min value was greater than max value");
            return Rng.Next(a, b);
        }

        private float RandF()
        {
            float b = this.Stack.Pop().ArgF;
            float a = this.Stack.Pop().ArgF;
            try
            {
                return a + (b - a) * (float)Rng.NextDouble();
            }
            catch (Exception e)
            {
                this.ThrowException(e.Message);
                return 0;
            }
        }

        public Color4 Int32ToColor(int n)
        {
            return new Color4((byte)(n >> 16), (byte)(n >> 8 & 0xff), (byte)(n & 0xff), (byte)0xff);
        }

        public void ThrowException(string message)
        {
            throw new StageScriptException(message, this.Script[this.Ptr], this.Ptr + 1);
        }
    }
}
