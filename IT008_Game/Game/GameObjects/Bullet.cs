using IT008_Game.Core.Components;

using IT008_Game.Core.System;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal class Bullet : GameObject
    {
        public readonly Sprite2D Sprite;
        float _bulletSpeed;
        Vector2 _moveVec;

        GameTimer timer1;
        public Bullet(Vector2 dir,float speed = 1500f)
        {
            _bulletSpeed = speed;

            Sprite = new Sprite2D(
                AssetsBundle.LoadImageBitmap("dino.png")
            );
            Sprite.Transform.Scale = new Vector2(0.3f, 0.3f);
            _moveVec = dir;

            timer1 = new GameTimer(1.5f, true);
            timer1.Timeout += Timer1_Timeout;
            timer1.Start();

            Children.Add(timer1);
        }

        private void Timer1_Timeout(object? sender, EventArgs e)
        {
            Destroy();
        }

        public void Setup(Vector2 Position)
        {
            Sprite.Transform.Position = Position;
        }

        public override void Update()
        {
            Sprite.Transform.Translate(_moveVec * _bulletSpeed * GameTime.DeltaTime);

            base.Update();
        }
        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);

            base.Draw(g);
        }
        public override void OnDestroy()
        {
            timer1.Timeout -= Timer1_Timeout;
            base.OnDestroy();
        }

    }
}
