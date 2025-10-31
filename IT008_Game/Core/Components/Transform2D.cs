using System.Drawing.Drawing2D;
using System.Numerics;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// An object that contains location, scale, rotation infos
    /// </summary>
    internal sealed class Transform2D
    {
        public Vector2 Position = Vector2.Zero; 
        public float RotationDeg  = 0f;
        public Vector2 Scale = Vector2.One;
        public Vector2 Pivot = Vector2.Zero; 

        public void Translate(Vector2 dv)
        {
            Position.X += dv.X;
            Position.Y += dv.Y;
        }

        public Matrix GetMatrix() {
            var m = new Matrix();

            m.Translate(Position.X, Position.Y, MatrixOrder.Append);
            m.Rotate(RotationDeg, MatrixOrder.Append);
            m.Scale(Scale.X, Scale.Y, MatrixOrder.Append);
            m.Translate(Pivot.X, Pivot.Y, MatrixOrder.Append);

            return m;
        }
    }
}
