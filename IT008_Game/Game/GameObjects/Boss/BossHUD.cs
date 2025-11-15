using IT008_Game.Core.Components;

namespace IT008_Game.Game.GameObjects.Boss
{
    internal class BossHUD : GameObject
    {
        IBoss _boss;

        public BossHUD(IBoss boss)
        {
            _boss = boss;
        }

        public override void Draw(Graphics g)
        {
            


        }
    }
}
