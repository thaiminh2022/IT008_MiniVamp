using IT008_Game.Core.Components;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal class AnimatedGuy : GameObject
    {
        public AnimatedSprite2D Sprite { get; private set; }
        public AnimatedGuy()
        {

            Sprite = new AnimatedSprite2D();

            Sprite.AddAnimation("player/idle.png", 
                "idle", 
                new AnimationConfig { 
                    TotalRow = 1, 
                    TotalColumn = 2 
                }
            );
            Sprite.AddAnimation("player/walk.png", "walk", new AnimationConfig
            {
                TotalRow = 1,
                TotalColumn = 4,
                FPS = 10,
            });

            Sprite.AddAnimation("player/smash.png", "smash", new AnimationConfig
            {
                TotalRow = 2,
                TotalColumn = 3,
                FrameOffset = 2,
                Speed = 0.5f,
            });


            Sprite.Transform.Position.X = 500;
            Sprite.Transform.Position.Y = 200;
            Sprite.Transform.Scale = new Vector2(3, 3);
            Sprite.Play("smash");
        }

        public override void Update()
        {
            base.Update();
            Sprite.Update();
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);

            g.DrawSprite(Sprite);
        }
    }
}
