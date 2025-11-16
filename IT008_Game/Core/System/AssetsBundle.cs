using System.Media;

namespace IT008_Game.Core.System
{
    internal static class AssetsBundle
    {
        public static Bitmap LoadImageBitmap(string path)
        {
            var image = Image.FromFile("Assets/" + path);
            return new Bitmap(image);
        }
    }
}
