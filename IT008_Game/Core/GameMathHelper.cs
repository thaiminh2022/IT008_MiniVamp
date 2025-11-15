using System.Numerics;

namespace IT008_Game.Core
{
    internal static class GameMathHelper
    {
        public static PointF ToPointF(this Vector2 v) => new PointF(v.X, v.Y);
        public static Vector2 ToVector2(this PointF p) => new Vector2(p.X, p.Y);

        public static Vector2 NextInsideUnitCircle(this Random rng)
        {
            // u1, u2 in [0, 1)
            double u1 = rng.NextDouble();
            double u2 = rng.NextDouble();

            // radius must be sqrt(u1) to be uniform in area
            double r = Math.Sqrt(u1);
            double theta = 2.0 * Math.PI * u2;

            float x = (float)(r * Math.Cos(theta));
            float y = (float)(r * Math.Sin(theta));

            return new Vector2(x, y);   // inside unit circle centered at (0,0)
        }
    }
}
