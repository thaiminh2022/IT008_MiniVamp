using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal sealed class HomingBullet : Bullet
    {
        private float _homingStrength = 10f;       // how fast we turn towards target (bigger = snappier)
        private Enemy? _lockTarget;
        private Vector2 _moveVec;

        public HomingBullet(Vector2 dir) :base(dir)
        {
            _bulletSpeed = 500f;
            // Fallback direction if dir == zero
            _moveVec = dir.LengthSquared() > 0
                ? Vector2.Normalize(dir)
                : new Vector2(1f, 0f);
        }

        public override void Update()
        {
            // 1. Try to ensure we have a valid target
            AcquireTargetIfNeeded();

            // 2. Adjust direction towards target if we have one
            if (_lockTarget is not null && IsEnemyValid(_lockTarget))
            {
                Vector2 toTarget = _lockTarget.Sprite.Transform.Position - Sprite.Transform.Position;
                float distSq = toTarget.LengthSquared();

                if (distSq > 1f) // avoid zero-length normalize
                {
                    Vector2 desiredDir = Vector2.Normalize(toTarget);

                    // Smooth homing: lerp current dir towards desired dir
                    float t = _homingStrength * GameTime.DeltaTime;
                    if (t > 1f) t = 1f;

                    _moveVec = Vector2.Normalize(Vector2.Lerp(_moveVec, desiredDir, t));
                }
            }
            else
            {
                // Target became invalid (died/removed) -> forget it, keep flying in last direction
                _lockTarget = null;
            }

            // 3. Move forward in our current direction
            Sprite.Transform.Translate(_moveVec * _bulletSpeed * GameTime.DeltaTime);

            Children.Update();
        }

        private void AcquireTargetIfNeeded()
        {
            // Already have a valid target -> keep it (no jitter from switching targets too often)
            if (_lockTarget is not null && IsEnemyValid(_lockTarget))
                return;

            if (SceneManager.CurrentScene is not MainGameScene mg)
                return;

            float nearestDistSq = float.MaxValue;
            Enemy? nearestEnemy = null;

            foreach (var obj in mg.EnemyList)
            {
                if (obj is not Enemy enemy)
                    continue;

                if (!IsEnemyValid(enemy))
                    continue;

                Vector2 diff = enemy.Sprite.Transform.Position - Sprite.Transform.Position;
                float distSq = diff.LengthSquared();

                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestEnemy = enemy;
                }
            }

            _lockTarget = nearestEnemy;
        }

        // Adjust this depending on your Enemy implementation
        private static bool IsEnemyValid(Enemy enemy)
        {
            // If you have IsDead/IsAlive or something, use it here.
            // For now assume it's valid if sprite exists.
            return enemy.Sprite is not null;
        }
    }
}
