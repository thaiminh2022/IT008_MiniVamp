using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using System.Net.Http.Headers;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class FireCircle : GameObject
    {
        public AnimatedSprite2D Sprite { get; private set; }

        private float _startUniformScale = 100f;
        private float _targetUnifromScale = 40f;

        private float totalTimeBeforeEnd = 5f;

        public FireCircle()
        {
            Sprite = new AnimatedSprite2D();
            Sprite.AddAnimation("boss2/fire_circle.png", "circle", new AnimationConfig
            {
                TotalColumn = 2,
                TotalRow = 1,
            });

            Sprite.Play("circle");
            Sprite.Transform.Position = new Vector2(
                GameManager.VirtualWidth / 2f,
                GameManager.VirtualHeight / 2f
                );
            Sprite.Transform.Scale = Vector2.One * _startUniformScale;
        }

        public override void Update()
        {
            if (Sprite.Transform.Scale.X > _targetUnifromScale)
            {
                var scalePerScond = (_startUniformScale - _targetUnifromScale) / totalTimeBeforeEnd;
                Sprite.Transform.Scale -= Vector2.One * scalePerScond * GameTime.DeltaTime;
                Console.WriteLine($"Smaller {Sprite.Transform.Scale}");
            }
            Sprite.Update();
            base.Update();
        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }
    }
}
