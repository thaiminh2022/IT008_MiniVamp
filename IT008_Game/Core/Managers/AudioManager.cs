using IT008_Game.Core.System;
using System.Media;

namespace IT008_Game.Core.Managers
{
    internal static class AudioManager
    {
        public static readonly SoundPlayer ShootSound;
        public static readonly SoundPlayer HitSound;

        static AudioManager()
        {
            ShootSound = AssetsBundle.LoadSound("shoot.wav");
            HitSound = AssetsBundle.LoadSound("hit.wav");
        }
    }
}
