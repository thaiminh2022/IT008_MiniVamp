using IT008_Game.Core.Components;

namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal class PlayerLevelSystem : ValueSystem<int>
    {
        private int _level;

        public int Level => _level;

        public PlayerLevelSystem(int expPerLevel) : base(expPerLevel)
        {
            _level = 1;
        }
       
        public override void AddValue(int amount)
        {
            base.AddValue(amount);

            if (_value >= _maxValue)
            {
                _level++;
                _value = 0;
            }
        }
        public void LevelUp()
        {
            _level++;
            _value = 0;
        }
    }
}
