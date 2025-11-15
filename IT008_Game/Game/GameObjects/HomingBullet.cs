using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal sealed class HomingBullet : Bullet
    {
        float _bulletSpeed = 1500f; // adjust speed
        Vector2 _moveVec;

        public HomingBullet(Vector2 dir) : base(dir)
        {
            _moveVec = Vector2.Normalize(dir); // initial direction
        }

        public override void Update()
        {
            // Get current scene
            if (SceneManager.CurrentScene is MainGameScene mg && this.Sprite != null && mg.EnemyList.Count > 0)
            {
                Enemy? nearest = null;
                float nearestDist = float.MaxValue;

                // Find nearest enemy
                foreach (var obj in mg.EnemyList)
                {
                    if (obj is Enemy enemy && enemy.Sprite != null)
                    {
                        float dist = Vector2.Distance(enemy.Sprite.Transform.Position, this.Sprite.Transform.Position);
                        if (dist < nearestDist)
                        {
                            nearestDist = dist;
                            nearest = enemy;
                        }
                    }
                }

                // Update move vector toward nearest enemy
                if (nearest != null)
                {
                    var toEnemy = nearest.Sprite.Transform.Position - this.Sprite.Transform.Position;
                    if (toEnemy.LengthSquared() > 0)
                        _moveVec = Vector2.Normalize(toEnemy);
                }
            }

            // Move bullet along updated vector
            if (this.Sprite != null)
            {
                this.Sprite.Transform.Position += _moveVec * _bulletSpeed * GameTime.DeltaTime;
            }

            base.Update();
        }
    }


}
