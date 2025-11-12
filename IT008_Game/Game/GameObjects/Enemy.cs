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
        public float Speed = 150f;

        private float ChargeTimer = 0f;
        public float ChargeTime = 1f; 

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
            ChargeTimer += GameTime.DeltaTime;
            if(ChargeTimer > ChargeTime)
            {
                ChargeTimer = 0;
                OnChargeTrigger();
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

        private void OnChargeTrigger()
        {
            Shoot(_target);
        }
        public void LinearChase(Sprite2D Target)
        {
            Vector2 ChaseDirection;

            if (Target == null) ChaseDirection = Vector2.Zero;
            else ChaseDirection = Vector2.Normalize(Target.Transform.Position - Sprite.Transform.Position);

            Sprite.Transform.Position += ChaseDirection * Speed * GameTime.DeltaTime;
        }

       public void Shoot(Sprite2D Target)
        {
            Vector2 MainShootDirection;

            if (Target == null) MainShootDirection = Vector2.Zero;
            else MainShootDirection = Vector2.Normalize(Target.Transform.Position - Sprite.Transform.Position);

            for (int i = -1; i <= 1; i++)
            {
                Vector2 ShootDirection = GameMathConverter.Rotate(MainShootDirection, i/2f);
                var bullet = new Bullet(ShootDirection);
                bullet.Setup(Sprite.Transform.Position + 50 * ShootDirection);
                if (SceneManager.CurrentScene is MainGameScene mg)
                {
                    mg.BulletList.Add(bullet);
                    
                }
            }
            AudioManager.ShootSound.Play();


        }
    }
}
