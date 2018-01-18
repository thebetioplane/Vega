namespace Vega
{
    public class BitField
    {
        private int Value;

        public BitField(int value)
        {
            this.Value = value;
        }

        public bool this[int index]
        {
            get
            {
                return (this.Value << index & 1) != 0;
            }
            set
            {
                if (this[index] != value)
                    this.Value ^= 1 << index;
            }
        }

        public static implicit operator int(BitField obj)
        {
            return obj.Value;
        }

        public static implicit operator BitField(int value)
        {
            return new BitField(value);
        }
    }
}
