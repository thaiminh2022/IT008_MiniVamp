using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Introduction
{
    internal class IntroductionBoss : GameObject, IEnemy
    {
        enum Attacks
        {
            Dash,
            MeleeAOE,
            RangeAOE,
        }

        AnimatedSprite2D Sprite;

        // Player collision with boss will be handled later
        Player _player;

        // Mechanics
        float startTimeBtwAttack = 1.8f;
        float timeBtwAttack = 1.8f;

        bool _attackCalled = false;
        bool _willDoubleAttack = false;
        Animation2D _atkAnim;

        // Stats

        public HealthSystem HealthSystem { get; private set; }

        // Start function
        public IntroductionBoss(Player player)
        {
            HealthSystem = new HealthSystem(500f);

            Sprite = new AnimatedSprite2D();

            Sprite.AddAnimation("boss/idle.png", "idle", new AnimationConfig
            {
                TotalColumn = 10,
                TotalRow = 1,
            });

            Sprite.AddAnimation("boss/run.png", "walk", new AnimationConfig
            {
                TotalColumn = 16,
                TotalRow = 1,
            });

            _atkAnim = Sprite.AddAnimation("boss/attack.png", "attack", new AnimationConfig
            {
                TotalColumn = 7,
                FPS = 15,
                TotalRow = 1,
                Loop = false,
            });

            Sprite.AddAnimation("boss/hurt.png", "hurt", new AnimationConfig
            {
                TotalColumn = 4,
                FPS = 4,
                TotalRow = 1,
                Loop = false,
            });

            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth - 100f, GameManager.VirtualHeight / 2f);
            Sprite.Transform.Scale = new Vector2(-1, 1) * 3;

            _player = player;
            timeBtwAttack = startTimeBtwAttack;

            Sprite.Play("idle");

            BossHUD hud = new BossHUD(HealthSystem, "Yamaguchi Epstein");
            Children.Add(hud);

            AudioManager.PlayBossFight1Music();

        }


        bool _playedDeadAnim = false;
        // Update
        public override void Update()
        {
            HandleDead();

            if (!HealthSystem.IsDead) 
                HandleAttack();

            // Keep this so its children will update
            base.Update();
            Sprite.Update();
        }

        private void HandleDead()
        {
            if (HealthSystem.IsDead)
            {
                if (!_playedDeadAnim)
                {
                    Sprite.Play("hurt");
                    _playedDeadAnim = true;
                }

                if (Sprite.AnimationFinished())
                {
                    Destroy();
                }
            }
        }

        private void HandleAttack()
        {
            if (timeBtwAttack <= 0)
            {
                if (!_attackCalled)
                {
                    var playerY = _player.Sprite.Transform.Position.Y;
                    var diff = playerY - Sprite.Transform.Position.Y;
                    if (Math.Abs(diff) > 5f)
                    {
                        var dir = diff switch
                        {
                            > 0 => 1,
                            < 0 => -1,
                            _ => 0
                        };

                        var speed = 500f;
                        Sprite.Transform.Translate(0, dir * speed * GameTime.DeltaTime);
                        Sprite.Play("walk");

                    }
                    else
                    {
                        Sprite.Play("attack");
                        _willDoubleAttack = new Random().NextDouble() > 0.4;
                        Sprite.Transform.Position = new Vector2(Sprite.Transform.Position.X,
                            _player.Sprite.Transform.Position.Y);

                        _attackCalled = true;
                    }
                }
            }

            var attackFrame = 4;
            if (_attackCalled &&
                Sprite.CurrentFrame == attackFrame &&
                Sprite.CurrentAnimation == _atkAnim
            )
            {
                var bossSlash = new IntroductionBossSlash(_player, Sprite.Transform.Position);
                Children.Add(bossSlash);
                _attackCalled = false;

                if (_willDoubleAttack)
                {
                    timeBtwAttack = 0.25f;
                }
                else
                {
                    timeBtwAttack = startTimeBtwAttack;
                }
                AudioManager.PlaySwordSlash();
            }
            else
            {
                if (Sprite.AnimationFinished())
                {
                    Sprite.Play("idle");
                }

                timeBtwAttack -= GameTime.DeltaTime;
            }
        }

        // Draw
        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);

            // Keep this so its children will draw
            base.Draw(g);
        }

        public int GetWeight()
        {
            return 100;
        }

        public Sprite2D GetSprite()
        {
            return Sprite;
        }

        public void Damage(float damage)
        {
            HealthSystem.SubstractValue(damage);
        }
    }
}
