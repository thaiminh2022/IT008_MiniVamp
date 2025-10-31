using IT008_Game.Core.Managers;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// An object that handles texture work
    /// </summary>
    internal class Sprite2D
    {
        public Transform2D Transform;
        public Bitmap Texture;
        
        public Sprite2D(Bitmap texture)
        {
            Texture = texture;
            Transform = new();
        }

        public RectangleF GetRectangleF()
        {
            // THIS DOES NOT WORK FFS
            //var pts = GetRectCorners(); // new corners

            //float minX = pts[0].X, maxX = pts[0].X;
            //float minY = pts[0].Y, maxY = pts[0].Y;

            //// find the top left, and bottom right corners position
            //for (int i = 1; i < pts.Length; i++)
            //{
            //    if (pts[i].X < minX) minX = pts[i].X;
            //    if (pts[i].X > maxX) maxX = pts[i].X;
            //    if (pts[i].Y < minY) minY = pts[i].Y;
            //    if (pts[i].Y > maxY) maxY = pts[i].Y;
            //}

            return new RectangleF(Transform.Position.ToPointF(), 
                new SizeF(Texture.Width * Transform.Scale.X, Texture.Height * Transform.Scale.Y));

        }
        public bool CollidesWith(Sprite2D s)
        {
            return GetRectangleF().IntersectsWith(s.GetRectangleF());
        }

        //TODO: FIX
        private PointF[] GetRectCorners()
        {
            var w = Texture.Width;
            var h = Texture.Height;

            PointF[] pts = [
                new PointF(0, 0),
                new PointF(w, 0),
                new PointF(0, h),
                new PointF(w, h),
            ];

            using var m = Transform.GetMatrix();
            m.TransformPoints(pts);
            return pts;
        }
    }

    internal static class GraphicsExtension
    {
        public static void DrawSprite(this Graphics g, Sprite2D s)
        {
            g.TranslateTransform(s.Transform.Position.X, s.Transform.Position.Y);
            g.RotateTransform(s.Transform.RotationDeg);
            g.ScaleTransform(s.Transform.Scale.X, s.Transform.Scale.Y);
            g.TranslateTransform(s.Transform.Pivot.X, s.Transform.Pivot.Y);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;


            g.DrawImage(s.Texture, 0, 0, s.Texture.Width, s.Texture.Height);

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
