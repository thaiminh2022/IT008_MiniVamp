using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT008_Game.Game.GameObjects.EnemyTypes
{
    internal class Enemy_Normal : Enemy
    {
        public Enemy_Normal(Player ChaseTarget) : base(ChaseTarget)
        {
        }
    }
}
