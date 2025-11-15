using IT008_Game.Core.Components;
using IT008_Game.Core.System;
namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal class PlayerHUD : GameObject
    {
        Player _player;

        float _wantToValue;
        float _lastValue;

        float _updateTime = .5f;

        public PlayerHUD(Player player)
        {
            _player = player;
            DrawLayer = 1000;

            _lastValue = 0;
            _wantToValue = _player.HealthSystem.GetValue();

            _player.HealthSystem.OnHealthChange += HealthSystem_OnHealthChange;

        }

        private void HealthSystem_OnHealthChange(object? sender, EventArgs e)
        {
            // React to healthChange
            _wantToValue = _player.HealthSystem.GetValue();
        }


        public override void Update()
        {
            if (_wantToValue != _lastValue)
            {
                var dist = Math.Abs(_wantToValue - _lastValue);
                var len = dist / _updateTime;

                var dir = _wantToValue - _lastValue switch
                {
                    < 0 => -1,
                    > 0 => 1,
                    _ => 0,
                };

                _lastValue += dir * len * GameTime.DeltaTime;

                if (dist < 0.2f)
                {
                    _lastValue = _wantToValue;
                }
            }


            base.Update();
        }
        public override void OnDestroy()
        {
             
            base.OnDestroy();
        }

        public override void Draw(Graphics g)
        {
            using var backBrush = new SolidBrush(Color.Gray);
            using var frontBrush = new SolidBrush(Color.Red);

            var barWidth = 500f;

            var backRect = new RectangleF(0, 0, barWidth, 25);

            var healthBarWidth = barWidth * _player.HealthSystem.GetValueNormalized();
            var healthBar = new RectangleF(0, 0, healthBarWidth, 25);

            g.FillRectangle(backBrush, backRect);
            g.FillRectangle(frontBrush, healthBar);
        }
    }
}
