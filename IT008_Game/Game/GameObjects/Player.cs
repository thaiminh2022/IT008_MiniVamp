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
        public float _dashDistance = 100f; // How far the player should dash
        private float _dashCooldown = 0.5f; // Time before dash can be used again
        private float _dashTimer = 0f;
        private Vector2 _lastMoveDir = new Vector2(1, 0);
        
        public float HP { get; set; } = 500f;
        public float MaxHP { get; private set; } = 500f;
        
        public bool _isInvulnerable = false;
        private float _invulnerableTimer = 0f;

        public int Level { get; private set; } = 1;

        public Player()
        {
            Sprite = new(
                AssetsBundle.LoadImageBitmap("dino.png")
            );
            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth / 2, GameManager.VirtualHeight / 2);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }


        public override void Update()
        {
            var rawInput = new Vector2(GameInput.GetAxis(Axis.Horizontal),
                               GameInput.GetAxis(Axis.Vertical));

            // Only update last move direction if there is input
            if (rawInput.LengthSquared() > 0)
            {
                _lastMoveDir = Vector2.Normalize(rawInput);

                // turning
                if (_lastMoveDir.X < 0)
                    Sprite.Transform.Scale = new Vector2(-Math.Abs(Sprite.Transform.Scale.X), Sprite.Transform.Scale.Y);
                else if (_lastMoveDir.X > 0)
                    Sprite.Transform.Scale = new Vector2(Math.Abs(Sprite.Transform.Scale.X), Sprite.Transform.Scale.Y);
            }

            // Moving
            var moveVec = rawInput * _speed * GameTime.DeltaTime;
            Sprite.Transform.Translate(moveVec);

            // Cooldown on dash
            if (_dashTimer > 0)
                _dashTimer -= GameTime.DeltaTime;
            
            // Dash
            if (GameInput.GetKeyDown(Keys.F) && _dashTimer <= 0)
            {
                var dashInput = new Vector2(GameInput.GetAxis(Axis.Horizontal),
                                            GameInput.GetAxis(Axis.Vertical));

                Vector2 dashDir = dashInput.LengthSquared() > 0
                    ? Vector2.Normalize(dashInput)
                    : _lastMoveDir;

                Sprite.Transform.Translate(dashDir * _dashDistance);
                _lastMoveDir = dashDir; // keep facing consistent
                _dashTimer = _dashCooldown;

                //immortal for a sec
                _isInvulnerable = true;
                _invulnerableTimer = 0.15f;
            }

            if (_isInvulnerable)
            {
                _invulnerableTimer -= GameTime.DeltaTime;
                if (_invulnerableTimer <= 0f)
                {
                    _isInvulnerable = false;
                }
            }

            // Shooting
            if (GameInput.GetKeyDown(Keys.Space) || GameInput.GetMouseButtonDown(MouseButtons.Left))
            {
                switch (Level)
                {
                    case 1:
                        // Get mouse position relative to game window
                        //System.Drawing.Point mouseScreen = System.Windows.Forms.Control.MousePosition;
                        System.Drawing.Point mouseClient = GameForm.MousePosition;
                        Vector2 mousePos = new Vector2(mouseClient.X, mouseClient.Y);

                        // Player position
                        Vector2 playerPos = Sprite.Transform.Position;

                        // Direction vector
                        Vector2 toMouse = mousePos - playerPos;
                        if (toMouse.LengthSquared() > 0)
                        {
                            Vector2 dir = Vector2.Normalize(toMouse);
                            SpawnBullet(dir);
                        }
                        break; ;

                    case 2:
                        // Shoot in 4 directions (up, down, left, right)
                        SpawnBullet(new Vector2(1, 0));
                        SpawnBullet(new Vector2(-1, 0));
                        SpawnBullet(new Vector2(0, 1));
                        SpawnBullet(new Vector2(0, -1));
                        break;

                    case 3:
                        // Shoot in 8 directions (like a star)
                        for (int i = 0; i < 8; i++)
                        {
                            float angle = MathF.PI / 4 * i; // 45° increments
                            var dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                            SpawnBullet(dir);
                        }
                        break;

                    case 4:
                        // Shoot one homing bullet
                        SpawnBullet(_lastMoveDir, homing: true);
                        break;
                    case 5:
                        SpawnBullet(Vector2.Normalize(new Vector2(1, 1)), homing: true);
                        SpawnBullet(Vector2.Normalize(new Vector2(-1, 1)), homing: true);
                        SpawnBullet(Vector2.Normalize(new Vector2(1, -1)), homing: true);
                        SpawnBullet(Vector2.Normalize(new Vector2(-1, -1)), homing: true);
                        break;
                    case 6:
                        for (int i = 0; i < 8; i++)
                        {
                            float angle = MathF.PI / 4 * i; // 45° increments
                            var dir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                            SpawnBullet(dir, homing: true);
                        }
                        break;
                }

                AudioManager.ShootSound.Play();
            }

            // Clamp player inside screen bounds
            var pos = Sprite.Transform.Position;

            // Half‑size of sprite (so we clamp by edges, not center)
            float halfW = (Sprite.Region.Width * MathF.Abs(Sprite.Transform.Scale.X)) / 2f;
            float halfH = (Sprite.Region.Height * MathF.Abs(Sprite.Transform.Scale.Y)) / 2f;

            // Clamp X between left and right edges
            pos.X = Math.Clamp(pos.X, halfW, GameManager.VirtualWidth - halfW);

            // Clamp Y between top and bottom edges
            pos.Y = Math.Clamp(pos.Y, halfH, GameManager.VirtualHeight - halfH);

            Sprite.Transform.Position = pos;

            if (HP <= 0)
            {
                SpawnExplosion();
                Destroy();
            }


            base.Update();
        }

        private void SpawnBullet(Vector2 dir, bool homing = false)
        {
            var bullet = homing ? new HomingBullet(dir) : new Bullet(dir);
            bullet.Setup(Sprite.Transform.Position);

            if (SceneManager.CurrentScene is MainGameScene mg)
                mg.BulletList.Add(bullet);
        }

        private void SpawnExplosion()
        {
            var explosion = new Explosion(Sprite.Transform.Position);
            if (SceneManager.CurrentScene is MainGameScene mg)
            {
                mg.Children.Add(explosion); // or mg.BulletList.Add(explosion) if you want it managed there
            }
        }


        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);

            SolidBrush green = new SolidBrush(Color.Green);
            SolidBrush gray = new SolidBrush(Color.Gray);
       
            // --- UI Health Bar ---
            float hpRatio = Math.Clamp(HP / MaxHP, 0f, 1f);

            // Position at bottom center of screen
            float barWidth = 500f;   // desired width in pixels
            float barHeight = 20f;   // desired height in pixels
            float x = (GameManager.VirtualWidth - barWidth) / 2f;
            float y = GameManager.VirtualHeight - barHeight - 75f; // 10px above bottom

            // Draw background (gray)
            g.FillRectangle(gray, x, y, barWidth, barHeight);

            // Draw foreground (red) scaled by HP
            g.FillRectangle(green, x, y, barWidth * hpRatio, barHeight);

            base.Draw(g);
        }

    }
}
