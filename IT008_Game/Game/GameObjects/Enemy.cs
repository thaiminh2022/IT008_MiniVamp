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
        public Sprite2D Sprite;
        public int MaxHealth = 10;
        public int CurrentHealth;
        public int Damage = 1;
        public float MovementSpeed = 50f;
        public short EnemyID = 0;

        //spawn multiple enemies vs single hard enemy
        public int EnemyWeight = 2;
        public float EnemyDiffLvl = 1;

        //AimingDirection
        private Vector2 AimingDirection = Vector2.Zero;

        //enemy attack cooldown
        private float ChargeTimer = 0f;
        public float ChargeTime = 3f; 

        //number off time enemy use attack when attacking
        public int AtkNum = 5;
        private int AtkCount = 0;

        //time between each attack
        private float AtkTimer = 0f;
        public float AtkTime = 0.3f;

        //player target
        private Sprite2D _target;

        public Enemy(Player? ChaseTarget,float DifficultyLvl = 1f)
        {
             _target = ChaseTarget.Sprite;
            CurrentHealth = MaxHealth;
            EnemyDiffLvl = DifficultyLvl;

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
            if (ChargeTimer <= ChargeTime) LinearChase();
           
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
            
            Vector2 ShootDirection = GameMathHelper.Rotate(AimingDirection,0);
            var bullet = new Bullet(ShootDirection,200);
            bullet.Setup(Sprite.Transform.Position + 50 * ShootDirection);
            if (SceneManager.CurrentScene is MainGameScene mg)
            {
                mg.EnemyBulletList.Add(bullet);
                    
            }
            AudioManager.ShootSound.Play();


        }
    }
}
