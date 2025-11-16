using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal sealed class Player : GameObject
    {
        public readonly Sprite2D Sprite;

        bool _dashAvailablePlayed = false;

        private float _timeBtwAttack = 0;
        private float _startTimeBtwAttack => 1 / AttackSpeed;
        public float AttackSpeed { get; set; } = 2f;
        public float Speed { get;  set; } = 200f;

        public float Damage { get; set; } = 200f;
        public float MaxHealth { 
            get => HealthSystem.GetMaxValue();
            set => HealthSystem.SetMaxValue(value, true); 
        }

        public float _dashDistance = 100f; // How far the player should dash
        public float DashCoolDown = 3f; // Time before dash can be used again
        public bool CanDash => _dashTimer <= 0;

        private float _dashTimer = 0f;
        private Vector2 _lastMoveDir = new Vector2(1, 0);

        public HealthSystem HealthSystem { get; private set; }
        public PlayerLevelSystem LevelSystem { get; private set; }


        private float _damageSoundCooldown = 0.5f;

        public Player()
        {
            HealthSystem = new HealthSystem(100);
            HealthSystem.OnDamage += HealthSystem_OnDamage;

            LevelSystem = new PlayerLevelSystem(100, this);
            Sprite = new(
                AssetsBundle.LoadImageBitmap("dino.png")
            );
            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth / 2, GameManager.VirtualHeight / 2);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);

            var hud = new PlayerHUD(this);
            Children.Add(hud);
        }

        private void HealthSystem_OnDamage(object? sender, EventArgs e)
        {
            if (_damageSoundCooldown <= 0)
            {
                AudioManager.PlayPlayerHit();
                _damageSoundCooldown = 0.5f;
            }
        }

        public override void OnDestroy()
        {
            HealthSystem.OnDamage -= HealthSystem_OnDamage;
            base.OnDestroy();
        }

        public override void Update()
        {
            if (_damageSoundCooldown > 0)
            {
                _damageSoundCooldown -= GameTime.DeltaTime;
            }

            if (_timeBtwAttack <= 0)
            {
                HandleShooting();
            }
            else
            {
                _timeBtwAttack -= GameTime.DeltaTime;
            }

            HandleMoving();
            HandleDashing();
            HandleClamping();

            if (HealthSystem.GetValue() <= 0)
            {
                SpawnExplosion();
                Destroy();
            }

            base.Update();
        }

        private void HandleClamping()
        {

            // Clamp player inside screen bounds
            var pos = Sprite.Transform.Position;

            // Half‑size of sprite (so we clamp by edges, not center)
            float halfW = Sprite.Region.Width * MathF.Abs(Sprite.Transform.Scale.X) / 2f;
            float halfH = Sprite.Region.Height * MathF.Abs(Sprite.Transform.Scale.Y) / 2f;

            // Clamp X between left and right edges
            pos.X = Math.Clamp(pos.X, halfW, GameManager.VirtualWidth - halfW);

            // Clamp Y between top and bottom edges
            pos.Y = Math.Clamp(pos.Y, halfH, GameManager.VirtualHeight - halfH);

            Sprite.Transform.Position = pos;
        }

        private void HandleShooting()
        {

            // Shooting
            if (GameInput.GetKeyDown(Keys.Space) || GameInput.GetMouseButtonDown(MouseButtons.Left))
            {
                switch (LevelSystem.Level)
                {
                    case 1:
                        var mousePos = GameInput.MousePosition;

                        // Player position
                        Vector2 playerPos = Sprite.Transform.Position;

                        // Direction vector
                        Vector2 toMouse = mousePos - playerPos;
                        if (toMouse.LengthSquared() > 0)
                        {
                            Vector2 dir = Vector2.Normalize(toMouse);
                            SpawnBullet(dir);
                        }
                        break;
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


                _timeBtwAttack = _startTimeBtwAttack;
                AudioManager.PlayShoot();
            }
        }
        private void HandleDashing()
        {
            // Cooldown on dash
            if (_dashTimer > 0)
            {
                _dashTimer -= GameTime.DeltaTime;
                _dashAvailablePlayed = false;
                return;
            }

            if (!_dashAvailablePlayed) {
                AudioManager.PlayPlayerDashAvailable();
                _dashAvailablePlayed = true;
            }

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
                _dashTimer = DashCoolDown;

                AudioManager.PlayPlayerDash();
            }
        }

        private void HandleMoving()
        {
            var rawInput = new Vector2(GameInput.GetAxis(Axis.Horizontal),
                               GameInput.GetAxis(Axis.Vertical));

            // Only update last move direction if there is input
            if (rawInput.LengthSquared() > 0)
            {
                _lastMoveDir = Vector2.Normalize(rawInput);
            }

            // Moving
            var moveVec = rawInput * Speed * GameTime.DeltaTime;
            Sprite.Transform.Translate(moveVec);
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
            var explosion = new PlayerExplosion(Sprite.Transform.Position);
            if (SceneManager.CurrentScene is MainGameScene mg)
            {
                mg.Children.Add(explosion); // or mg.BulletList.Add(explosion) if you want it managed there
            }
        }


        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }

    }
}
