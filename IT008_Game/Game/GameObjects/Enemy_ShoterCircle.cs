using IT008_Game.Core.Components;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Core.Managers;
using IT008_Game.Game.Scenes;
using System.Numerics;
using IT008_Game.Core.System;

namespace IT008_Game.Game.GameObjects.EnemyTypes
{
    internal class Enemy_ShooterCircle : Enemy
    {
        public new int EnemyWeight = 8;

        private readonly int BulletCount = 12;
        private readonly float BulletSpeed = 220f;

        public Enemy_ShooterCircle(Player target, float diff = 1f)
            : base(target, diff)
        {
            HealthSystem = new HealthSystem(80 * diff);
            Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("creep/witch.png"));
            Sprite.Transform.Scale = new Vector2(0.9f, 0.9f);
        }

        public override void LinearChase()
        {
            // enemy just stand, not chase

        }

        public override void Shoot()
        {
            if (!(SceneManager.CurrentScene is MainGameScene mg)) return;

            float angleStep = 360f / BulletCount;

            for (int i = 0; i < BulletCount; i++)
            {
                float angleRad = MathF.PI * 2 * (i / (float)BulletCount);
                Vector2 dir = new Vector2(MathF.Cos(angleRad), MathF.Sin(angleRad));

                Bullet b = new Bullet(dir, BulletSpeed);
                b.Setup(Sprite.Transform.Position + dir * 40);
                mg.EnemyBulletList.Add(b);
            }

            AudioManager.PlayShoot();

        }
    }
}
