using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

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
