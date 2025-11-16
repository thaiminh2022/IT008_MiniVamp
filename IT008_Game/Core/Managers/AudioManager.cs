using IT008_Game.Core.System;
using System.Media;

namespace IT008_Game.Core.Managers
{
    internal static class AudioManager
    {
        private static readonly SoundPlayer ShootSound;
        private static readonly SoundPlayer HitSound;

        static AudioManager()
        {
            ShootSound = AssetsBundle.LoadSound("shoot.wav");
            HitSound = AssetsBundle.LoadSound("hit.wav");
        }

        
        public static void PlayShoot()
        {
            try { ShootSound?.Play(); }
            catch { }
        }

        
        public static void PlayHit()
        {
            try { HitSound?.Play(); }
            catch { }
        }
    }
}
