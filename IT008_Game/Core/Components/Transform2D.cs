using System.Drawing.Drawing2D;
using System.Numerics;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// An object that contains location, scale, rotation infos
    /// </summary>
    internal sealed class Transform2D
    {
        public float RotationDeg { get; set; } = 0f;
        public Vector2 ScaleABS => new Vector2(Math.Abs(Scale.X), Math.Abs(Scale.Y));


        private Vector2 _position = Vector2.Zero;
        private Vector2 _scale = Vector2.One;
        public Vector2 Position { get => _position; set => _position = value; }
        public Vector2 Scale { get => _scale; set {
                Pivot /= _scale;
                _scale = value;
                Pivot *= value;
        }}
        public Vector2 Pivot { get; set; } = Vector2.Zero;

        public void Translate(Vector2 dv)
        {
            Translate(dv.X, dv.Y);
        }
        public void Translate(float dx, float dy)
        {
            _position.X += dx;
            _position.Y += dy;
        }

        public Matrix GetMatrix()
        {
            var m = new Matrix();

            m.Translate(Position.X, Position.Y, MatrixOrder.Append);
            m.Rotate(RotationDeg, MatrixOrder.Append);
            m.Scale(Scale.X, Scale.Y, MatrixOrder.Append);
            m.Translate(Pivot.X, Pivot.Y, MatrixOrder.Append);

            return m;
        }
    }
}
