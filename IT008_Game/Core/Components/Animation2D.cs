namespace IT008_Game.Core.Components
{
    internal record AnimationConfig
    {
        public required int TotalRow;
        public required int TotalColumn;

        /// <summary>
        /// Remove x amount of frame from the total frame
        /// </summary>
        public int FrameOffset = 0;
        public bool Loop = true;
        public int FPS = 8;
        public float Speed = 1;
    }

    internal class Animation2D
    {
        // Have to set
        public Bitmap Texture { get; private set; }
        public int TotalFrame { get; private set; }

        public float FrameWidth { get; private set; }
        public float FrameHeight { get; private set; }

        // Optional set
        public AnimationConfig Config { get; private set; }

        public Animation2D(Bitmap texture, AnimationConfig config)
        {
            Texture = texture;
            Config = config;

            TotalFrame = config.TotalRow * config.TotalColumn - config.FrameOffset;

            FrameWidth = (float)texture.Width / config.TotalColumn;
            FrameHeight = (float)texture.Height / config.TotalRow;

        }
        public RectangleF GetFrameRect(int frame)
        {
            if (frame >= TotalFrame)
            {
                throw new Exception("Frame not setup correctly");
            }

            int x = frame % Config.TotalColumn;
            int y = frame / Config.TotalColumn;

            // Console.WriteLine($"({x},{y})");

            return new RectangleF(
                x * FrameWidth,
                y * FrameHeight,
                FrameWidth,
                FrameHeight
            );
        }

        public float GetSecondsPerFrame()
        {
            return 1f / Config.FPS;
        }
    }
}
