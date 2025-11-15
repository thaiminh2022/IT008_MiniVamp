using IT008_Game.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IT008_Game.Game.GameObjects.EnemyTypes
{
    internal class Enemy_Normal : Enemy
    {
        public Enemy_Normal(Player ChaseTarget) : base(ChaseTarget)
        {
            int size = 100;
            Bitmap redSquare = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(redSquare))
            {
                g.Clear(Color.Orange);
            }


            Sprite = new Sprite2D(redSquare);
            Sprite.Transform.Position = new Vector2(800, 200);
            Sprite.Transform.Scale = new Vector2(0.5f, 0.5f);
        }
    }
}
