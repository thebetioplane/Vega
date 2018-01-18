namespace Vega.GameModePlay.StageScript
{
    public class ScriptStack
    {
        private readonly int Size;
        private ScriptStackEntry[] Arr;
        private int Ptr;
        private StageScriptRunner Parent;

        public ScriptStackEntry Top
        {
            get
            {
                if (this.Ptr == -1)
                    this.Parent.ThrowException("Stack underflow");
                if (this.Ptr == this.Size)
                    this.Parent.ThrowException("Stack overflow");
                return this.Arr[this.Ptr];
            }
        }

        public ScriptStack(StageScriptRunner parent, int size)
        {
            this.Parent = parent;
            this.Ptr = -1;
            this.Size = size;
            this.Arr = new ScriptStackEntry[size];
        }

        public void Push(ScriptStackEntry obj)
        {
            this.Ptr++;
            if (this.Ptr == this.Size)
                this.Parent.ThrowException("Stack overflow");
            this.Arr[this.Ptr] = obj;
        }

        public ScriptStackEntry Pop()
        {
            if (this.Ptr == -1)
                this.Parent.ThrowException("Stack underflow");
            return this.Arr[this.Ptr--];
        }

        public float Cmp()
        {
            if (this.Top.IsFloat)
                return this.Top.ArgF;
            else
                return this.Top.ArgI;
        }

        public void Dec()
        {
            if (this.Top.IsFloat)
                this.Top.ArgF--;
            else
                this.Top.ArgI--;
        }

        public void Inc()
        {
            if (this.Top.IsFloat)
                this.Top.ArgF++;
            else
                this.Top.ArgI++;
        }

        public void Neg()
        {
            if (this.Top.IsFloat)
                this.Top.ArgF = -this.Top.ArgF;
            else
                this.Top.ArgI = -this.Top.ArgI;
        }

        private byte Arith(out ScriptStackEntry a, out ScriptStackEntry b)
        {
            b = this.Pop();
            a = this.Top;
            return (byte)(((a.IsFloat ? 1 : 0) << 1) | (b.IsFloat ?  1 : 0));
        }

        public void Add()
        {
            ScriptStackEntry b;
            ScriptStackEntry a;
            switch (this.Arith(out a, out b))
            {
                case 0:
                    a.ArgI += b.ArgI;
                    break;
                case 1:
                    a.ToFloat();
                    a.ArgF += b.ArgF;
                    break;
                case 2:
                    a.ArgF += b.ArgI;
                    break;
                case 3:
                    a.ArgF += b.ArgF;
                    break;
            }
        }

        public void Sub()
        {
            ScriptStackEntry b;
            ScriptStackEntry a;
            switch (this.Arith(out a, out b))
            {
                case 0:
                    a.ArgI -= b.ArgI;
                    break;
                case 1:
                    a.ToFloat();
                    a.ArgF -= b.ArgF;
                    break;
                case 2:
                    a.ArgF -= b.ArgI;
                    break;
                case 3:
                    a.ArgF -= b.ArgF;
                    break;
            }
        }

        public void Mult()
        {
            ScriptStackEntry b;
            ScriptStackEntry a;
            switch (this.Arith(out a, out b))
            {
                case 0:
                    a.ArgI *= b.ArgI;
                    break;
                case 1:
                    a.ToFloat();
                    a.ArgF *= b.ArgF;
                    break;
                case 2:
                    a.ArgF *= b.ArgI;
                    break;
                case 3:
                    a.ArgF *= b.ArgF;
                    break;
            }
        }

        public void Div()
        {
            ScriptStackEntry b;
            ScriptStackEntry a;
            switch (this.Arith(out a, out b))
            {
                case 0:
                    a.ArgI /= b.ArgI;
                    break;
                case 1:
                    a.ToFloat();
                    a.ArgF /= b.ArgF;
                    break;
                case 2:
                    a.ArgF /= b.ArgI;
                    break;
                case 3:
                    a.ArgF /= b.ArgF;
                    break;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i <= this.Ptr; i++)
            {
                sb.Append(this.Arr[i].ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
