using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class FireCircle : GameObject
    {
        Player _player;


        private float _currentRadius = 2000f;
        private float _targetRadius = 450;

        private float updateSpeed;

        Vector2 origin;

        public FireCircle(Player player)
        {
            _player = player;
            var time = 5f;
            updateSpeed = (_currentRadius - _targetRadius) / time;
            origin = new Vector2(GameManager.VirtualWidth / 2f, GameManager.VirtualHeight / 2f);
        }

        public override void Update()
        {
            if (_currentRadius > _targetRadius)
            {
                _currentRadius -= updateSpeed * GameTime.DeltaTime;
                // Console.WriteLine($"Smaller {Sprite.Transform.Scale}");
            }
            if (_currentRadius <= _targetRadius)
            {

                var dist = Vector2.Distance(origin, _player.Sprite.Transform.Position);
                if (dist >= _targetRadius)
                {
                    // Damage player
                    _player.HealthSystem.SubstractValue(5 * GameTime.DeltaTime);
                }
            }

            base.Update();
        }

        public override void Draw(Graphics g)
        {
            var fullRect = new Rectangle(0, 0, GameManager.VirtualWidth, GameManager.VirtualHeight);
            PointF center = origin.ToPointF();
            float radius = _currentRadius;
            float diameter = radius * 2f;

            using var path = new GraphicsPath();
            path.AddEllipse(center.X - radius, center.Y - radius, diameter, diameter);

            using var region = new Region(fullRect);

            int alpha = (int)(0.4f * 255);
            Color overlay = Color.FromArgb(alpha, Color.IndianRed);
            using var br = new SolidBrush(overlay);

            region.Exclude(path);
            g.FillRegion(br, region);

            base.Draw(g);
        }
    }
}
