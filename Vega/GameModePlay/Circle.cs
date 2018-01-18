using OpenTK;

namespace Vega.GameModePlay
{
    public class Circle
    {
        public readonly Vector2 Center;
        public readonly float Radius;

        public Circle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public Circle()
            : this(Vector2.Zero, 0.0f)
        {
        }

        public Circle(float x, float y, float radius)
            : this(new Vector2(x, y), radius)
        {
        }

        public Circle(Sprite obj)
        {
            this.Center = Vector2.Zero;
            this.Radius = (obj.Width + obj.Height) / 4.0f;
        }

        /*
        public bool HitTest(Circle other)
        {
            float a = this.Center.X - other.Center.X;
            float b = this.Center.Y - other.Center.Y;
            float c = this.Radius + other.Radius;
            return a * a + b * b <= c * c;
        }
        */
    }
}
