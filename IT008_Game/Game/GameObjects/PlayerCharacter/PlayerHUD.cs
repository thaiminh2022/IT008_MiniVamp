using IT008_Game.Core.Components;
using IT008_Game.Core.System;
using System.Drawing.Text;
namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal class PlayerHUD : GameObject
    {
        Player _player;

        float _wantToValue;
        float _lastValue;

        float _updateSpeed = 100f;

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
                float maxDelta = _updateSpeed * GameTime.DeltaTime;
                float diff = _wantToValue - _lastValue;

                if (Math.Abs(diff) <= maxDelta)
                {
                    _lastValue = _wantToValue;
                }
                else
                {
                    _lastValue += Math.Sign(diff) * maxDelta;
                }
                //Console.WriteLine($"want: {_wantToValue}, last: {_lastValue}, diff: {diff}, sign: {MathF.Sign(diff)}");

            }


            base.Update();
        }
        public override void OnDestroy()
        {
            _player.HealthSystem.OnHealthChange -= HealthSystem_OnHealthChange;
            base.OnDestroy();
        }

        public override void Draw(Graphics g)
        {
            DrawHealthBar(g);

            var levelStr = $"Level: {_player.LevelSystem.Level}";
            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            g.DrawString(levelStr, font, brush, new PointF(0, 25));
        }

        private void DrawHealthBar(Graphics g)
        {
            using var backBrush = new SolidBrush(Color.Gray);
            using var frontBrush = new SolidBrush(Color.Red);

            var barWidth = 500f;

            var backRect = new RectangleF(0, 0, barWidth, 25);

            var healthBarWidth = barWidth * (_lastValue / _player.HealthSystem.GetMaxValue());
            var healthBar = new RectangleF(0, 0, healthBarWidth, 25);

            g.FillRectangle(backBrush, backRect);
            g.FillRectangle(frontBrush, healthBar);
        }
    }
}
