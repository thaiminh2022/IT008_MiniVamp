using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IT008_Game.Core
{
    internal static class GameMathConverter
    {
        public static PointF ToPointF(this Vector2 v) => new PointF(v.X, v.Y);
        public static Vector2 ToVector2(this PointF p) => new Vector2(p.X, p.Y);

        public static Vector2 Rotate(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            return new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
        }

    }
}
