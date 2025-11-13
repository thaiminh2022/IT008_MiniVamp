using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;
namespace IT008_Game.Game.GameObjects
{
    internal sealed class Player : GameObject
    {
        Enemy other;
        public readonly Sprite2D Sprite;
        private float _speed = 200f;
        public float _dashDistance = 80f; // How far the player should dash
        private float _dashCooldown = 0.5f; // Time before dash can be used again
        private float _dashTimer = 0f;
        private Vector2 _lastMoveDir = new(1, 0);
        public float HP = 500f;
        public Player()
        {
            Sprite = new(
            AssetsBundle.LoadImageBitmap("dino.png")
            );
            Sprite.Transform.Pivot = new Vector2(
                -Sprite.Region.Width / 2f,
                -Sprite.Region.Height / 2f
            );
            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth / 2, GameManager.VirtualHeight / 2);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }
        public override void Update()
        {
            var inputVector = new Vector2(GameInput.GetAxis(Axis.Horizontal),
            GameInput.GetAxis(Axis.Vertical));
            //rotate
            if (inputVector.Length() > 0)
            {
                _lastMoveDir = inputVector;
                float angle = MathF.Atan2(inputVector.Y, inputVector.X) * (180f / MathF.PI);
                
                Sprite.Transform.RotationDeg = angle;
                if (_lastMoveDir.X < 0)
                    Sprite.Transform.Scale.Y = -Math.Abs(Sprite.Transform.Scale.Y);
                else if (_lastMoveDir.X > 0)
                    Sprite.Transform.Scale.Y = Math.Abs(Sprite.Transform.Scale.Y);
            }
            inputVector *= _speed * GameTime.DeltaTime;
            Sprite.Transform.Translate(inputVector);
            //dashes
            if (_dashTimer > 0)
            {
                _dashTimer -= GameTime.DeltaTime;
            }
            if (GameInput.GetKeyDown(Keys.E) && _dashTimer <= 0)
            {
                var dashDir = Vector2.Normalize(_lastMoveDir);
                Sprite.Transform.Translate(dashDir * _dashDistance);
                _dashTimer = _dashCooldown;
            }
            //shoots
            if (GameInput.GetKeyDown(Keys.Space) || GameInput.GetMouseButtonDown(MouseButtons.Left))
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
