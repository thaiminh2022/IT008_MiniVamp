using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal class Explosion : GameObject
    {
        private readonly Sprite2D sprite;
        private int currentFrame = 0;
        private float frameTimer = 0f;

        // 5x6 grid over a 480x480 PNG -> each frame is 96x80
        private const int columns = 5;
        private const int rows = 5;
        private const int totalFrames = 30;
        private const int frameWidth = 96;
        private const int frameHeight = 96;
        private const float frameDuration = 0.03f; // ~33 FPS

        public Explosion(Vector2 position)
        {
            var texture = AssetsBundle.LoadImageBitmap("explosion.png");
            sprite = new Sprite2D(texture);

            // Set initial frame region BEFORE first draw to avoid flashing whole sheet
            sprite.Region = new Rectangle(0, 0, frameWidth, frameHeight);

            // Optional: center the sprite visually at the position
            sprite.Transform.Position = position - new Vector2(48f, 40f);

            // Optional: scale up/down without changing the region
            sprite.Transform.Scale = new Vector2(1f, 1f);
        }

        public override void Update()
        {
            frameTimer += GameTime.DeltaTime;
            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration; // subtract, not zero, to keep timing tight
                currentFrame++;

                if (currentFrame >= totalFrames)
                {
                    Destroy();
                    return;
                }

                int col = currentFrame % columns;
                int row = currentFrame / rows;
                sprite.Region = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
            }
        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(sprite); // must respect sprite.Region; ensure your Draw uses the source rect
        }
    }
}
