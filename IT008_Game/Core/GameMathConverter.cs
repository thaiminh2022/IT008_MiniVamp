using System.Numerics;

namespace IT008_Game.Core
{
    internal static class GameMathConverter
    {
        public static PointF ToPointF(this Vector2 v) => new PointF(v.X, v.Y);
        public static Vector2 ToVector2(this PointF p) => new Vector2(p.X, p.Y);
    }
}
