using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.GameObjects.PlayerCharacter;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class SecondaryBoss : GameObject, IEnemy
    {
        public readonly AnimatedSprite2D Sprite;
        private readonly Player _player;
        public HealthSystem HealthSystem { get; private set; }

        float _uniformScale = 7;

        enum State
        {
            Idle,
            SlamAttack,
            BiteAttack,
            LimitPlayer,
            TwoWaySlash,
            Death,
        }
        State _currentState;
        int _sameStateCount = 0;

        // IDLE
        float _timeBtwIdle = 0;
        float _startTimeBtwIdle = 3f;


        // SLAM ATTACK
        bool _didSlamed = false;
        Random _rng = new Random();


        // Bite attack
        bool _didBite = false;
        bool _moveToPlayer = false;

        // Limit player
        bool _didFireCircle = false;


        public SecondaryBoss(Player player)
        {
            Sprite = new AnimatedSprite2D();
            HealthSystem = new HealthSystem(4500);
            Sprite.AddAnimation("boss2/idle.png", "idle", new AnimationConfig
            {
                TotalColumn = 6,
                TotalRow = 1,
            });

            Sprite.AddAnimation("boss2/slam.png", "slam", new AnimationConfig
            {
                TotalColumn = 8,
                TotalRow = 1,
                Loop = false,
            });


            Sprite.AddAnimation("boss2/attack.png", "bite", new AnimationConfig
            {
                TotalColumn = 11,
                TotalRow = 1,
                FPS = 10,
                Loop = false,
            });

            Sprite.AddAnimation("boss2/death.png", "death", new AnimationConfig
            {
                TotalColumn = 10,
                TotalRow = 1,
                Loop = false,
            });

            _currentState = State.Idle;
            _timeBtwIdle = _startTimeBtwIdle;



            Sprite.Transform.Scale = new Vector2(1, 1) * _uniformScale;
            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth / 2f, GameManager.VirtualHeight / 2f);
            _player = player;

            var hud = new BossHUD(HealthSystem, "P Daddy's Slime");
            Children.Add(hud);

            AudioManager.PlayBossFight2Music();
        }

        public override void Update()
        {
            HandleLooking();

            if (HealthSystem.IsDead)
            {
                _currentState = State.Death;
            }

            switch (_currentState)
            {
                case State.Idle:
                    Sprite.Play("idle");
                    IdleState();
                    break;
                case State.SlamAttack:
                    Sprite.Play("slam");
                    SlamState();
                    break;
                case State.BiteAttack:
                    BiteState();
                    break;
                case State.LimitPlayer:
                    LimitPlayer();
                    break;
                case State.TwoWaySlash:
                    Sprite.Play("slam");
                    TwoWaySlash();
                    break;
                case State.Death:
                    if (WillDestroyNextFrame)
                        break;

                    Sprite.Play("death");
                    if (Sprite.AnimationFinished()) {
                        Destroy();
                    }
                    break;

            }

            Sprite.Update();
            base.Update();
        }

        private void TwoWaySlash()
        {
            if (!_didSlamed)
            {
                AudioManager.PlayBubble();
            }
            _didSlamed = true;

            if (_didSlamed && Sprite.AnimationFinished())
            {
                var slash = new TwoWaySlash(Sprite.Transform.Position, _player);
                Children.Add(slash);
                _currentState = State.Idle;

                AudioManager.PlaySlam();
            }

        }
        private void LimitPlayer()
        {
            if (_didFireCircle) return;

            _didFireCircle = true;
            var fire = new FireCircle(_player);
            Children.Add(fire);

            _currentState = State.Idle;
        }

        private void BiteState()
        {
            if (!_moveToPlayer && Vector2.Distance(_player.Sprite.Transform.Position, Sprite.Transform.Position) > 16f)
            {
                var dir = _player.Sprite.Transform.Position - Sprite.Transform.Position;
                dir = Vector2.Normalize(dir);

                Sprite.Transform.Position += dir * 500f * GameTime.DeltaTime;
                return;
            }
            Sprite.Play("bite");
            _moveToPlayer = true;


            var impactFrame = 7;
            if (!_didBite && Sprite.CurrentFrame == impactFrame)
            {
                // impact
                _didBite = true;

                AudioManager.PlayBite();

                if (HealthSystem.GetValueNormalized() <= .75f)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var proj = new TornadoProjectile(RandomPosition(true, 300f), _player);
                        Children.Add(proj);
                    }
                }


                var dist = Vector2.Distance(Sprite.Transform.Position, _player.Sprite.Transform.Position);
                var radius = 164f;
                if (dist <= radius)
                {
                    // damage player
                    _player.HealthSystem.SubstractValue(30f);

                }
            }


            if (Sprite.AnimationFinished())
            {
                _currentState = State.Idle;
            }
        }
        private void SlamState()
        {
            if (!_didSlamed)
            {
                AudioManager.PlayBubble();
            }
            
            _didSlamed = true;

            


            if (_didSlamed && Sprite.AnimationFinished())
            {
                for (int i = 0; i < 10; i++)
                {
                    var proj = new SlamAttackProjectile(_player);
                    proj.Sprite.Transform.Position = RandomPosition();

                    Children.Add(proj);
                }

                _currentState = State.Idle;
                AudioManager.PlaySlam();
            }
        }

        private void IdleState()
        {
            if (_timeBtwIdle <= 0)
            {
                if (!_didFireCircle && HealthSystem.GetValueNormalized() < .4f)
                {
                    _currentState = State.LimitPlayer;
                }
                else
                {
                    _currentState = ChooseState();
                }


                _didSlamed = false;
                _didBite = false;
                _moveToPlayer = false;

                _startTimeBtwIdle = _rng.Next(1, 2);
                _timeBtwIdle = _startTimeBtwIdle;
            }
            else
            {
                _timeBtwIdle -= GameTime.DeltaTime;
            }
        }

        private void HandleLooking()
        {
            if (_player.Sprite.Transform.Position.X < Sprite.Transform.Position.X)
            {
                Sprite.Transform.Scale = new Vector2(1, 1) * _uniformScale;
            }
            else if (_player.Sprite.Transform.Position.X > Sprite.Transform.Position.X)
            {
                Sprite.Transform.Scale = new Vector2(-1, 1) * _uniformScale;
            }
        }

        public override void Draw(Graphics g)
        {
            if (GameManager.DebugMode)
            {
                g.DrawCircle(Sprite.Transform.Position.ToPointF(), 164, new Pen(Color.AliceBlue, 3));
            }

            base.Draw(g);
            g.DrawSprite(Sprite);
        }

        private Vector2 RandomPosition(bool fromPlayer = false, float radius = 500f)
        {
            var pos = _rng.NextInsideUnitCircle() * radius;
            return pos + (fromPlayer ? _player.Sprite.Transform.Position : Sprite.Transform.Position);
        }

        private State ChooseState()
        {
            State[] normalStates =
            [
                State.SlamAttack,
                State.SlamAttack,
                State.BiteAttack,
                State.BiteAttack,
                State.BiteAttack,
                State.TwoWaySlash
            ];

            // Limit player will be use when boss <= 25% health
            int index = _rng.Next(normalStates.Length);
            var state = normalStates[index];


            // Cant have the same attack 2 times
            if (state == _currentState)
            {
                if (_sameStateCount < 2)
                {
                    _sameStateCount++;
                }
                else
                {
                    // Choose a new state until diff
                    while (state == _currentState)
                    {
                        index = _rng.Next(normalStates.Length);
                        state = normalStates[index];
                    }
                    _sameStateCount = 0;
                }
            }

            return normalStates[index];
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
