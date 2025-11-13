using IT008_Game.Core.Components;
using IT008_Game.Core.System;

namespace IT008_Game.Game.GameObjects.Spawner
{
    internal class TestSpawnerEnemy : GameObject
    {
        public Sprite2D Sprite;

        public TestSpawnerEnemy()
        {
            Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("dino.png"));
        }

        public override void Update()
        {
            if (GameInput.GetKeyDown(Keys.C))
            {
                Destroy();
            }


            base.Update();
        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }
    }
}
