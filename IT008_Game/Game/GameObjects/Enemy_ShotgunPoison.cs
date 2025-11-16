    using IT008_Game.Core.Components;
    using IT008_Game.Core.Managers;
    using IT008_Game.Core.System;
    using IT008_Game.Game.GameObjects.PlayerCharacter;
    using IT008_Game.Game.Scenes;
    using System.Numerics;

    namespace IT008_Game.Game.GameObjects.EnemyTypes
    {
        internal class Enemy_ShotgunPoison : Enemy_Shotgun
        {
            public Enemy_ShotgunPoison(Player target, float diff = 1f) : base(target, diff)
            {
                Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("poison_slime.png"));

                Sprite.Transform.Scale = new Vector2(1.0f, 1.0f);
            }
            public void DestroyEnemy()
            {
                if (SceneManager.CurrentScene is MainGameScene mg)
                {   
                    PoisonZone pz = new PoisonZone(Sprite.Transform.Position);
                    mg.EnemyBulletList.Add(pz);

                }
                base.Destroy();
            }
        

        }
    }
