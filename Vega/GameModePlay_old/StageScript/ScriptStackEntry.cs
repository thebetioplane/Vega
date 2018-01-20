namespace Vega.GameModePlay.StageScript
{
    public class ScriptStackEntry
    {
        public int ArgI { get; set; }
        public float ArgF { get; set; }
        public bool IsFloat { get; set; }

        public ScriptStackEntry(int i, float f, bool isFloat)
        {
            this.ArgI = i;
            this.ArgF = f;
            this.IsFloat = isFloat;
        }

        public override string ToString()
        {
            if (this.IsFloat)
                return this.ArgF.ToString() + 'f';
            else
                return this.ArgI.ToString();
        }

        public void ToFloat()
        {
            this.IsFloat = true;
            this.ArgF = (float)this.ArgI;
        }

        public void ToInt()
        {
            this.IsFloat = false;
            this.ArgI = (int)this.ArgF;
        }
    }
}
