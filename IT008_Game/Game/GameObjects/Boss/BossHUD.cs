using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;

namespace IT008_Game.Game.GameObjects.Boss
{
    internal class BossHUD : GameObject
    {

        HealthSystem _bossSystem;
        string _bossName;

        float _wantToValue;
        float _lastValue;

        float _updateSpeed = 500f;

        public BossHUD(HealthSystem bossSystem, string bossName = "")
        {
            _bossSystem = bossSystem;
            _bossName = bossName;

            _updateSpeed = bossSystem.GetMaxValue();

            _lastValue = 0;
            _wantToValue = _bossSystem.GetMaxValue();
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
        }

        public override void Draw(Graphics g)
        {
            DrawTitle(g);
            DrawHealthBar(g);
        }

        private void DrawTitle(Graphics g)
        {
            var barWidth = 700f;
            var titlePosition = new PointF(
                GameManager.VirtualWidth / 2f - barWidth / 2f,
                GameManager.VirtualHeight - 75f
            );
            using var font = new Font("Segoe UI", 10, FontStyle.Bold);
            using var brush = new SolidBrush(Color.Black);

            g.DrawString(_bossName, font, brush, titlePosition);
        }

        private void DrawHealthBar(Graphics g)
        {
            var barWidth = 700f;

            var barPosition = new PointF(
                GameManager.VirtualWidth / 2f - barWidth / 2f,
                GameManager.VirtualHeight - 50f
            );


            using var backBrush = new SolidBrush(Color.Gray);
            using var frontBrush = new SolidBrush(Color.DarkRed);


            var backRect = new RectangleF(barPosition, new SizeF(barWidth, 25));

            var normalize = _bossSystem.GetMaxValue() == 0 ?
                                0 : _lastValue / _bossSystem.GetMaxValue();

            var healthBarWidth = barWidth * normalize;
            var healthBar = new RectangleF(barPosition, new SizeF(healthBarWidth, 25));

            g.FillRectangle(backBrush, backRect);
            g.FillRectangle(frontBrush, healthBar);
        }
    }
}
