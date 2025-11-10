using System.Diagnostics;

namespace IT008_Game.Core.System
{
    internal class GameTime
    {
        private static readonly Stopwatch _stopWatch = Stopwatch.StartNew();
        private static double _last;


        // local times
        public static float DeltaTime { get; set; }
        public static float TimeScale { get; set; } = 1;
        public static float Time { get; set; }

        public static void Tick()
        {
            // Calculating deltatime
            var now = _stopWatch.Elapsed.TotalSeconds;
            var dt = now - _last;
            _last = now;

            // if (dt > 0.25) dt = 0.25;


            DeltaTime = (float)dt * TimeScale;
            Time += DeltaTime;
        }
    }

}
