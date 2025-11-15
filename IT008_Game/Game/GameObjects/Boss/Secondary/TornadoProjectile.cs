using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class TornadoProjectile : GameObject
    {
        Player _player;

        public readonly AnimatedSprite2D Sprite;

        Vector2 _spawnPosition;
        Vector2 _moveToPosition;

        float _timeBtwNewPosition = 3;
        float _startTimeBtwNewPosition = 3;

        float totalAliveTime = 10f;

        public TornadoProjectile(Vector2 spawnPosition, Player player)
        {
            _player = player;
            Sprite = new AnimatedSprite2D();
            Sprite.AddAnimation("boss2/tornado.png", "tornado", new AnimationConfig
            {
                TotalColumn = 4,
                TotalRow = 1,
                FPS = 12,
            });
            Sprite.Play("tornado");


            _spawnPosition = spawnPosition;
            _moveToPosition = _spawnPosition;
            Sprite.Transform.Scale = Vector2.One * 3;
            Sprite.Transform.Position = spawnPosition;
        }


        public override void Update()
        {
            if (totalAliveTime <= 0)
            {
                Destroy();
                return;
            }
            totalAliveTime -= GameTime.DeltaTime;

            if (_timeBtwNewPosition <= 0)
            {
                if (_moveToPosition == Sprite.Transform.Position)
                {
                    var rand = new Random();
                    _moveToPosition = rand.NextInsideUnitCircle() * 100f;
                    _moveToPosition += Sprite.Transform.Position;
                }

                if (Vector2.Distance(_moveToPosition, Sprite.Transform.Position) > 10f)
                {
                    var dir = _moveToPosition - Sprite.Transform.Position;
                    dir = Vector2.Normalize(dir);
                    Sprite.Transform.Position += dir * 100f * GameTime.DeltaTime;
                }
                else
                {
                    _moveToPosition = Sprite.Transform.Position;
                    _timeBtwNewPosition = _startTimeBtwNewPosition;
                }

            }
            else
            {
                _timeBtwNewPosition -= GameTime.DeltaTime;
            }

            if (Sprite.CollidesWith(_player.Sprite))
            {
                _player.HealthSystem.SubstractValue(3 * GameTime.DeltaTime);
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
