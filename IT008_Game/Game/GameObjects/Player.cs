using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal sealed class Player : GameObject
    {
        public readonly Sprite2D Sprite;
        private float _speed = 200f;

        public Player() {
            Sprite = new (
                AssetsBundle.LoadImageBitmap("dino.png")
            );
            Sprite.Transform.Position = new Vector2(100, 100);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }


        public override void Update()
        {
            var inputVector = new Vector2(GameInput.GetAxis(Axis.Horizontal), 
                GameInput.GetAxis(Axis.Vertical));

            inputVector *= _speed * GameTime.DeltaTime;
            Sprite.Transform.Translate(inputVector);

            if (GameInput.GetKeyDown(Keys.Space))
            {
                var bullet = new Bullet(new Vector2(1, 0));
                bullet.Setup(Sprite.Transform.Position);

                if (SceneManager.CurrentScene is MainGameScene mg)
                {
                    mg.BulletList.Add(bullet);
                    AudioManager.ShootSound.Play();
                }
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
