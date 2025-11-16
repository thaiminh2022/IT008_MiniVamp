using System.Numerics;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using IT008_Game.Game.Scenes;

namespace IT008_Game.Game.GameObjects.EnemyTypes
{
    internal class PoisonZone : GameObject
    {
        private float Duration = 4f;
        private float DamagePerSecond = 8f;
        private float Radius = 90f;
        private Sprite2D Sprite;

        public PoisonZone(Vector2 pos)
        {
            Bitmap bmp = new Bitmap((int)Radius, (int)Radius);
            using (Graphics g = Graphics.FromImage(bmp))
                g.Clear(Color.FromArgb(120, Color.Green));

            Sprite = new Sprite2D(bmp);
            Sprite.Transform.Position = pos;
        }

        public override void Update()
        {
            base.Update();

            // Lay scene hien tai va cast ve MainGameScene
            var mg = SceneManager.CurrentScene as MainGameScene;
            if (mg == null) return;

            foreach (var obj in mg.EnemyList)
            {
                if (obj is IEnemy enemy)
                {
                    var enemySprite = enemy.GetSprite();
                    if (enemySprite.CollidesWith(Sprite))
                    {
                        enemy.Damage(1);
                    }
                }
            }
        }


        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }
    }
}
