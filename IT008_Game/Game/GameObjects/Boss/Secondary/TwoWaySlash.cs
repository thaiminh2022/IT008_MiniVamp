using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class TwoWaySlash : GameObject
    {
        public AnimatedSprite2D LeftSlash, RightSlash;

        Player _player;

        private float timeBtwDestroy = 5f;

        public TwoWaySlash(Vector2 origin, Player player)
        {
            LeftSlash = new AnimatedSprite2D();
            RightSlash = new AnimatedSprite2D();

            _player = player;

            LeftSlash.AddAnimation("boss2/twoway.png", "slash", new AnimationConfig
            {
                TotalColumn = 3,
                TotalRow = 1,
            });
            LeftSlash.Transform.Scale = new Vector2(-1, 1);

            RightSlash.AddAnimation("boss2/twoway.png", "slash", new AnimationConfig
            {
                TotalColumn = 3,
                TotalRow = 1,
            });

            LeftSlash.Transform.Position = origin + new Vector2(0, -50);
            RightSlash.Transform.Position = origin + new Vector2(0, -50);

            LeftSlash.Transform.Scale = new Vector2(-1, 1) * 5;
            RightSlash.Transform.Scale = Vector2.One * 5;


            LeftSlash.Play("slash");
            RightSlash.Play("slash");
        }

        public override void Update()
        {
            var _speed = 500f;
            LeftSlash.Transform.Translate(-_speed * GameTime.DeltaTime, 0f);
            RightSlash.Transform.Translate(_speed * GameTime.DeltaTime, 0f);

            LeftSlash.Update();
            RightSlash.Update();


            if (LeftSlash.CollidesWith(_player.Sprite) && !WillDestroyNextFrame)
            {
                // Deal damage;
                _player.HealthSystem.SubstractValue(10f);
                Destroy();
            }
            if (RightSlash.CollidesWith(_player.Sprite) && !WillDestroyNextFrame)
            {

                // Deal damage;
                _player.HealthSystem.SubstractValue(10f);
                Destroy();
            }

            if (timeBtwDestroy > 0)
            {
                timeBtwDestroy -= GameTime.DeltaTime;
            }
            else
            {
                Destroy();
            }

            base.Update();
        }
        public override void Draw(Graphics g)
        {
            g.DrawSprite(LeftSlash);
            g.DrawSprite(RightSlash);
            base.Draw(g);
        }
    }
}
