using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class SlamAttackProjectile : GameObject
    {
        public readonly AnimatedSprite2D Sprite;

        private float waitTime;

        bool dealDamage = false;

        Player _player;

        public SlamAttackProjectile(Player player)
        {
            _player = player;
            Sprite = new AnimatedSprite2D();
            Sprite.AddAnimation("boss2/whirl.png", "whirl", new AnimationConfig
            {
                TotalColumn = 4,
                TotalRow = 1,
                FPS = 16,
            });

            Sprite.AddAnimation("boss2/whirl_explode.png", "whirl_explode", new AnimationConfig
            {
                TotalColumn = 4,
                TotalRow = 1,
                FPS = 14,
                Loop = false,
            });
            Sprite.Play("whirl");

            Sprite.Transform.Scale = Vector2.One * 4;
            waitTime = 3f;
        }
        public override void Update()
        {
            if (dealDamage)
                return;

            if (waitTime <= 0)
            {
                if (Sprite.CollidesWith(_player.Sprite))
                {
                    _player.HealthSystem.SubstractValue(10f);
                }

                Sprite.Play("whirl_explode");
                if (Sprite.AnimationFinished())
                {
                    Destroy();
                    dealDamage = true;
                }
            }
            else
            {
                waitTime -= GameTime.DeltaTime;
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
