namespace Vega
{
    public struct SlideValue
    {
        public float Curr;
        public float Dest;

        public SlideValue(float curr, float dest)
        {
            this.Curr = curr;
            this.Dest = dest;
        }

        public void Update()
        {
            this.Curr += (this.Dest - this.Curr) / 20.0f;
        }
    }
}
