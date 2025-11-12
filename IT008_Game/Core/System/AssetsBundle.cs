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
        public static SoundPlayer LoadSound(string path)
        {
            var sound = new SoundPlayer("Assets/" + path);
            return sound;
        }
    }
}
