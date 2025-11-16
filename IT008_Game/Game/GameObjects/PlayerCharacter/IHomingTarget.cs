using IT008_Game.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal interface IHomingTarget
    {
        Sprite2D GetSprite();
        bool GetIsAlive();
    }
}
