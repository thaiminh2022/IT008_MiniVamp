using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss.Secondary
{
    internal class SecondaryBoss : GameObject
    {
        public readonly AnimatedSprite2D Sprite;
        public readonly Player _player;

        float _uniformScale = 7;

        enum State
        {
            Idle,
            SlamAttack,
            BiteAttack,
            LimitPlayer,
            TwoWaySlash,
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
                FPS = 12, 
                Loop = false,
            });

            _currentState = State.Idle;
            _timeBtwIdle = _startTimeBtwIdle;



            Sprite.Transform.Scale = new Vector2(1, 1) * _uniformScale;
            Sprite.Transform.Position = new Vector2(GameManager.VirtualWidth / 2f, GameManager.VirtualHeight / 2f);
            _player = player;
        }

        public override void Update()
        {
            HandleLooking();

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

            }


            Sprite.Update();
            base.Update();
        }

        private void TwoWaySlash() {
            _didSlamed = true;

            if (_didSlamed && Sprite.AnimationFinished())
            {
                var slash = new TwoWaySlash(Sprite.Transform.Position, _player);
                Children.Add(slash);
                _currentState = State.Idle;
            }

        }
        private void LimitPlayer()
        {
            if (_didFireCircle) return;
            
            _didFireCircle = true;
            var fire = new FireCircle();
            Children.Add(fire);   
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
                for (int i = 0; i < 5; i++)
                {
                    var proj = new TornadoProjectile(RandomPosition());
                    Children.Add(proj);
                }

                var dist = Vector2.Distance(Sprite.Transform.Position, _player.Sprite.Transform.Position);
                var radius = 128f;
                if (dist <= radius)
                {
                   // damage player
                }
            }
           

            if (Sprite.AnimationFinished())
            {
                _currentState = State.Idle;
            }
        }
        private void SlamState()
        {
            _didSlamed = true;

            if (_didSlamed && Sprite.AnimationFinished())
            {
                for (int i = 0; i < 5; i++)
                {
                    var proj = new SlamAttackProjectile();
                    proj.Sprite.Transform.Position = RandomPosition();

                    Children.Add(proj);
                }

                _currentState = State.Idle;
            }
        }

        private void IdleState()
        {
            if (_timeBtwIdle <= 0)
            {
                _currentState = ChooseState();
                _didSlamed = false;
                _didBite = false;
                _moveToPlayer = false;
                _didFireCircle = false;

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
            base.Draw(g);
            g.DrawSprite(Sprite);
        }

        private Vector2 RandomPosition(bool fromPlayer = false)
        {
            var pos = _rng.NextInsideUnitCircle() * 500f;
            return pos + (fromPlayer ? _player.Sprite.Transform.Position : Sprite.Transform.Position);
        }

        private State ChooseState()
        {
            State[] normalStates =
            [
                State.SlamAttack,
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
                }else
                {
                    // Choose a new state until diff
                    while(state == _currentState)
                    {
                        index = _rng.Next(normalStates.Length);
                        state = normalStates[index];
                    }
                    _sameStateCount = 0;
                }
            }

            return normalStates[index];
        }

    }
}
