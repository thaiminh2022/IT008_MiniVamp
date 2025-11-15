using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Introduction
{
    internal class IntroductionBossSlash : GameObject
    {
        Sprite2D Sprite;

        float _speed = 500f;
        Player _player;

        float _damage = 10f;


        public IntroductionBossSlash(Player player, Vector2 position)
        {
            Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("boss/slash.png"));
            Sprite.Transform.Position = position;
            Sprite.Transform.Scale = new Vector2(1, 3);

            _player = player;
        }

        public override void Update()
        {
            Sprite.Transform.Translate(-_speed * GameTime.DeltaTime, 0);

            if (Sprite.CollidesWith(_player.Sprite) && WillDestroyNextFrame == false)
            {
                _player.HealthSystem.SubstractValue(_damage);
                Destroy();
            }

            base.Update();
        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);

            base.Draw(g);
        }
    }
}
