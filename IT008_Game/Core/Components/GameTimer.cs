using IT008_Game.Core.System;
namespace IT008_Game.Core.Components
{
    [Obsolete("Please use a timer in your code, not this")]
    internal sealed class GameTimer : GameObject
    {
        public float Interval { get; set; }
        public bool OneShot { get; set; }
        public float TimeLeft { get; private set; }

        public bool Paused { get; private set; }
        private readonly bool _destroyOnStop;

        public event EventHandler<EventArgs>? Timeout;

        public GameTimer(float interval, bool oneShot = true, bool destroyOnStop = false)
        {
            Interval = interval;
            OneShot = oneShot;
            _destroyOnStop = destroyOnStop;
        }

        public override void Update()
        {
            if (Paused) return;


            TimeLeft -= GameTime.DeltaTime;
            if (TimeLeft <= 0)
            {
                Timeout?.Invoke(this, EventArgs.Empty);
                TimeLeft = Interval;

                if (OneShot) Stop();
            }
        }

        public void Start()
        {
            TimeLeft = Interval;
            Paused = false;
        }

        public void Stop()
        {
            Paused = true;
            TimeLeft = Interval;

            if (_destroyOnStop) Destroy();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("I got destroyed");
        }

    }
}
