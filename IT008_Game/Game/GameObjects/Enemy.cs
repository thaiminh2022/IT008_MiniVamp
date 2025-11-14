using IT008_Game.Core;
using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using IT008_Game.Game.Scenes;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal class Enemy : GameObject
    {
        public readonly Sprite2D Sprite;
        public int MaxHealth = 10;
        public int CurrentHealth;
        public int Damage = 1;
        public float MovementSpeed = 150f;
        public short EnemyID;

        //spawn multiple enemies vs single hard enemy
        public int EnemyWeight;

        //AimingDirection
        private Vector2 AimingDirection = Vector2.Zero;

        //enemy attack cooldown
        private float ChargeTimer = 0f;
        public float ChargeTime = 1f; 

        //number off time enemy use attack when attacking
        public int AtkNum = 20;
        private int AtkCount = 0;

        //time between each attack
        private float AtkTimer = 0f;
        public float AtkTime = 0.05f;

        //player target
        private Sprite2D _target;

        public Enemy(Player ChaseTarget)
        {
            _target = ChaseTarget.Sprite;
            CurrentHealth = MaxHealth;

            int size = 100;
            Bitmap redSquare = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(redSquare))
            {
                g.Clear(Color.Red);
            }


            Sprite = new Sprite2D(redSquare);
            Sprite.Transform.Position = new Vector2(800, 200);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }

        public void Damaged()
        {
            AudioManager.HitSound.Play();
            CurrentHealth--;

        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }

        private void ClockWork()
        {
            if (ChargeTimer <= ChargeTime) {
                ChargeTimer += GameTime.DeltaTime;
                if (ChargeTimer > ChargeTime) OnChargeTrigger();
                return;
            }
            
            if(AtkTimer <= AtkTime && AtkCount >= 1)
                AtkTimer += GameTime.DeltaTime;
            else
            {
                if (AtkCount >= AtkNum)
                {
                    AtkCount = 0;
                    ChargeTimer = 0f;
                    return;
                }
                OnAttackTrigger();
                AtkTimer = 0f;
                AtkCount++;
                
            }
            
        }
        public override void Update()
        {
            ClockWork();
            //LinearChase(_target);
           
            if (CurrentHealth < 0)
            {
                Destroy();
            }
        }

        private void OnAttackTrigger()
        {
            Shoot();
        }

        public void LinearChase()
        {
            Vector2 ChaseDirection;

            if (_target == null) ChaseDirection = Vector2.Zero;
            else ChaseDirection = Vector2.Normalize(_target.Transform.Position - Sprite.Transform.Position);

            Sprite.Transform.Position += ChaseDirection * MovementSpeed * GameTime.DeltaTime;
        }

        public void OnChargeTrigger()
        {
            AimingDirection = Vector2.Normalize(_target.Transform.Position - Sprite.Transform.Position);
        }


        public void Shoot()
        {
            
            Vector2 ShootDirection = GameMathConverter.Rotate(AimingDirection,0);
            var bullet = new Bullet(ShootDirection);
            bullet.Setup(Sprite.Transform.Position + 50 * ShootDirection);
            if (SceneManager.CurrentScene is MainGameScene mg)
            {
                mg.EnemyBulletList.Add(bullet);
                    
            }
            AudioManager.ShootSound.Play();


        }
    }
}
