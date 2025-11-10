using IT008_Game.Core.Managers;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// An object that handles texture work
    /// </summary>
    internal class Sprite2D
    {
        public Transform2D Transform;
        public RectangleF Region;
        private Bitmap? _texture;
        public Bitmap? Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                if (_texture is null) return;

                Region.Width = _texture.Width;
                Region.Height = _texture.Height;
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
            {
                return Rectangle.Empty;
            }

            return new RectangleF(Transform.Position.ToPointF(),
                new SizeF(Region.Width * Transform.Scale.X, Region.Height * Transform.Scale.Y));

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

            g.TranslateTransform(s.Transform.Position.X, s.Transform.Position.Y);
            g.RotateTransform(s.Transform.RotationDeg);
            g.ScaleTransform(s.Transform.Scale.X, s.Transform.Scale.Y);
            g.TranslateTransform(s.Transform.Pivot.X, s.Transform.Pivot.Y);


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
            }

        }
    }
}
