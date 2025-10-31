using IT008_Game.Core.Components;
using IT008_Game.Core.Managers;
using IT008_Game.Core.System;
using System.Numerics;

namespace IT008_Game.Game.GameObjects
{
    internal class Enemy : GameObject
    {
        public readonly Sprite2D Sprite;

        public Enemy()
        {
            Sprite = new Sprite2D(AssetsBundle.LoadImageBitmap("dino.png"));
            Sprite.Transform.Position = new Vector2(800, 200);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }

        public void Damage()
        {
            AudioManager.HitSound.Play();
        }

        public override void Draw(Graphics g)
        {
            g.DrawSprite(Sprite);
            base.Draw(g);
        }
    }
}
