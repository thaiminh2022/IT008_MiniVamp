using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Core.Managers;
using IT008_Game.Game.Scenes;
using System.Numerics;
using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using IT008_Game.Core;

namespace IT008_Game.Game.GameObjects.EnemyTypes
{
    internal class Enemy_Shotgun : Enemy
    {
        private float StopDistance = 220f;
        private float SpreadDeg = 45f;
        private int PelletCount = 5;
        private float BulletSpeed = 250f;

        public Enemy_Shotgun(Player target, float diff = 1f) : base(target, diff)
        {
            Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("slime.png"));
            Sprite.Transform.Scale = new Vector2(1.0f, 1.0f);
        }

        public override void LinearChase()
        {
            if (_target == null) return;

            Vector2 dir = _target.Transform.Position - Sprite.Transform.Position;
            float dist = dir.Length();

            if (dist > StopDistance)
            {
                dir = Vector2.Normalize(dir);
                Sprite.Transform.Position += dir * MovementSpeed * GameTime.DeltaTime;
            }
        }

        public override void Shoot()
        {
            if (!(SceneManager.CurrentScene is MainGameScene mg)) return;

            Vector2 facing = Vector2.Normalize(_target.Transform.Position - Sprite.Transform.Position);

            float start = -SpreadDeg * 0.5f;
            float step = SpreadDeg / (PelletCount - 1);

            for (int i = 0; i < PelletCount; i++)
            {
                float angle = (start + step * i) * MathF.PI / 180f;
                Vector2 dir = GameMathHelper.Rotate(facing, angle);

                Bullet b = new Bullet(dir, BulletSpeed);
                b.Setup(Sprite.Transform.Position + dir * 40);
                mg.EnemyBulletList.Add(b);
            }

            AudioManager.ShootSound.Play();
        }
    }
}
