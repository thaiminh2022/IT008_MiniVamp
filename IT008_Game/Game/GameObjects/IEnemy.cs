using IT008_Game.Core.Components;

namespace IT008_Game.Game.GameObjects
{
    internal interface IEnemy
    {
        public int GetWeight();
        public Sprite2D GetSprite();
        public void Damage(float damage);
    }
}
