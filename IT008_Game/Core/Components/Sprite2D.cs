using IT008_Game.Core.Managers;
using System.Numerics;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// An object that handles texture work
    /// </summary>
    internal class Sprite2D
    {
        public Transform2D Transform;
        private RectangleF _region;

        public RectangleF Region
        {
            get { return _region; }
            set { _region = value;
                var size = (_region.Size / 2f).ToVector2();
                Transform.Pivot = size * Transform.Scale;
            }
        }

        private Bitmap? _texture;
        public Bitmap? Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                if (_texture is null) return;

                _region.Width = _texture.Width;
                _region.Height = _texture.Height;

                var size = (_region.Size / 2f).ToVector2();
                Transform.Pivot = size * Transform.Scale;
            }
        }

        public Sprite2D(Bitmap? texture)
        {
            _texture = texture;
            Transform = new();
            Region = new Rectangle(0, 0, Texture?.Width ?? 0, Texture?.Height ?? 0);
        }

        public RectangleF GetRectangleF()
        {
            if (Texture is null)
                return Rectangle.Empty;

            // Source size (unscaled)
            float srcW = _region.Width;
            float srcH = _region.Height;

            // Actual scale (can be negative to represent flips)
            Vector2 s = Transform.Scale;        // e.g., (-1, 1) for horizontal flip
            float w = srcW * MathF.Abs(s.X);    // final positive width
            float h = srcH * MathF.Abs(s.Y);    // final positive height

            // Base (unscaled) anchor
            Vector2 baseTopLeft = Transform.Position - Transform.Pivot;

            // If axis is flipped (scale < 0), shift the top-left back by the full size on that axis
            float left = baseTopLeft.X + (s.X >= 0 ? 0f : -w);
            float top = baseTopLeft.Y + (s.Y >= 0 ? 0f : -h);

            return new RectangleF(left, top, w, h);

        }
        public bool CollidesWith(Sprite2D s)
        {
            if (Texture is null || s.Texture is null)
                return false;


            return GetRectangleF().IntersectsWith(s.GetRectangleF());
        }
    }

    internal static class GraphicsExtension
    {
        public static void DrawSprite(this Graphics g, Sprite2D s)
        {
            if (s.Texture is null)
                return;

            // var drawPos = s.Transform.Position - s.Transform.Pivot;
            g.TranslateTransform(s.Transform.Position.X, s.Transform.Position.Y);
            g.RotateTransform(s.Transform.RotationDeg);
            g.ScaleTransform(s.Transform.Scale.X, s.Transform.Scale.Y);

            var pivot = s.Transform.Pivot / s.Transform.Scale;
            g.TranslateTransform(-pivot.X, -pivot.Y);

            g.DrawImage(s.Texture,
                new RectangleF(0, 0, s.Region.Width, s.Region.Height),
                s.Region,
                GraphicsUnit.Pixel
            );

            g.ResetTransform();

            // Debug
            if (GameManager.DebugMode)
            {
                using var pen = new Pen(Color.Red, 3f);
                g.DrawRectangle(pen, s.GetRectangleF());
                g.FillEllipse(new SolidBrush(Color.Green), s.Transform.Position.X, s.Transform.Position.Y, 10, 10);
                g.FillEllipse(new SolidBrush(Color.Yellow), 200, 200, 10, 10);
            }

        }
    }
}
