using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using System;
using System.Drawing;
using System.Numerics;

namespace IT008_Game.Game.GameObjects.Boss
{
    internal class IntroductionBoss : GameObject
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
        GameTimer _attackTimer;
        Random _random;

        Attacks? _nextAttack;
        private float _attackWindup = 0;

        // Values
        float _speed = 100f;
        bool _willChasePlayer = true;


        private Vector2 _dashToPos = Vector2.Zero;

        // Start function
        public IntroductionBoss(Player player) { 
            Sprite = new AnimatedSprite2D();

            Sprite.AddAnimation("player/idle.png", "idle", new AnimationConfig { 
                TotalColumn = 2, 
                TotalRow = 1
            });
            Sprite.AddAnimation("player/walk.png", "walk", new AnimationConfig
            {
                TotalColumn = 4,
                TotalRow = 1
            });
            Sprite.Transform.Position = new Vector2(200, 200);
            Sprite.Transform.Scale = new Vector2(3, 3);

            _random = new Random();
            _player = player;

            _attackTimer = new GameTimer(2);
            _attackTimer.Timeout += AttackTimer_Timeout;
            _attackTimer.Start();

            Children.Add(_attackTimer);

            Sprite.Play("walk");
        }

        private void AttackTimer_Timeout(object? sender, EventArgs e) 
        {
            var randAttack = _random.Next(0, 3);
            _nextAttack = Attacks.MeleeAOE;
            _willChasePlayer = false;
            _attackWindup = _nextAttack switch
            {
                Attacks.Dash => 1,
                Attacks.MeleeAOE => 2,
                Attacks.RangeAOE => 2,
                _ => throw new NotImplementedException(),
            };

            _dashToPos = _player.Sprite.Transform.Position;
        }
       
        // Update
        public override void Update()
        {
            if (_willChasePlayer)
            {
                var playerTrasnform = _player.Sprite.Transform;
                var dist = Vector2.Distance(playerTrasnform.Position, Sprite.Transform.Position);

                if (dist > Sprite.Region.Width / 2f)
                {
                    var moveVec = (_player.Sprite.Transform.Position - Sprite.Transform.Position);
                    Sprite.Transform.Position += Vector2.Normalize(moveVec) * _speed * GameTime.DeltaTime;
                }
            }
            if (Sprite.CollidesWith(_player.Sprite))
            {
                // Player take damage
                // player.health -= 1 * GameTime.DeltaTime;
            }

            if (_nextAttack is not null)
            {
                if (_attackWindup <= 0)
                {
                    // Do attack
                    Console.WriteLine(_nextAttack);

                    switch (_nextAttack)
                    {
                        case Attacks.Dash:
                            Sprite.Transform.Position = _dashToPos;
                            break;
                        case Attacks.MeleeAOE:
                            var sRect = Sprite.GetRectangleF();
                            var origin = new Vector2(sRect.Width / 2, sRect.Height / 2);
                            origin += Sprite.Transform.Position;

                            var dist = Vector2.Distance(origin, _player.Sprite.Transform.Position);
                            var attackRadius = 100f;
                            if (dist <= attackRadius)
                            {
                                Console.WriteLine("player hit");
                                // player.Heath -= x;
                            }
                            break;

                        case Attacks.RangeAOE:
                            // projectile = new Projectile(_player)
                            // projectile.Sprite.Transform.Position = Sprite.Transform.Position;
                            // Children.Add(projectile)
                            Console.WriteLine("bullets shot");
                            break;
                    }

                    _willChasePlayer = true;
                    _attackTimer.Start();
                    _nextAttack = null;
                }else
                {
                    // Charging attack
                    _attackWindup -= GameTime.DeltaTime;
                }
            }


            Sprite.Update();
            // Keep this so its children will update
            base.Update();
        }

        // Draw
        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);

            if (GameManager.DebugMode)
            {
                using var pen = new Pen(Color.Red);

                // Debug attack radius
                var sRect = Sprite.GetRectangleF();
                var origin = new Vector2(sRect.Width / 2, sRect.Height /2);
                origin += Sprite.Transform.Position;
                var radius = 100f;
                var rect = new RectangleF(origin.X - radius, origin.Y - radius, radius * 2f, radius * 2f);
                g.DrawEllipse(pen, rect);

                // Debug dash point
                g.FillEllipse(new SolidBrush(Color.Red), 
                    new RectangleF(_dashToPos.ToPointF(), new SizeF(10, 10)));
            }

            // Keep this so its children will draw
            base.Draw(g);
        }

        public override void OnDestroy()
        {
            _attackTimer.Timeout -= AttackTimer_Timeout;   
            
            
            base.OnDestroy();
        }
    }
}
