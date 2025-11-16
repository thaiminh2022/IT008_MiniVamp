using IT008_Game.Core.Components;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;

namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    internal class PlayerLevelSystem : ValueSystem<int>
    {
        private int _level;

        public int Level => _level;
        public event EventHandler? LevelUp;
        Player _player;

        List<PlayerUpgrade> _currentUpgrades;

        public PlayerLevelSystem(int expPerLevel, Player player) : base(expPerLevel)
        {
            _level = 1;
            _currentUpgrades = [];
            _player = player;
        }

        public override void AddValue(int amount)
        {
            base.AddValue(amount);

            if (_value >= _maxValue)
            {
                AddLevel();
            }
        }
        public void AddLevel()
        {
            _level++;
            _value = 0;
            LevelUp?.Invoke(this, EventArgs.Empty);
        }

        public List<PlayerUpgrade> GetUpgrades()
        {
            var op1 = PlayerUpgrade.GetRandom();
            var op2 = PlayerUpgrade.GetRandom();
            var op3 = PlayerUpgrade.GetRandom();
            _currentUpgrades.AddRange([op1, op2, op3]);
            return _currentUpgrades;
        }

        public void SelectUpgrade(int idx)
        {
            if (_currentUpgrades.Count == 0)
                return;

            var up = _currentUpgrades[idx];

            switch (up.Option)
            {
                case UpgradeOption.AttackSpeed:
                    _player.AttackSpeed = HandleUpgrade(_player.AttackSpeed, up);
                    break;
                case UpgradeOption.BaseDamage:
                    _player.Damage = HandleUpgrade(_player.Damage, up);
                    break;
                case UpgradeOption.MaxHealth:
                    _player.MaxHealth = HandleUpgrade(_player.MaxHealth, up);
                    break;
                case UpgradeOption.MoveSpeed:
                    _player.Speed = HandleUpgrade(_player.Speed, up);
                    break;
                case UpgradeOption.DashTime:
                    _player.DashCoolDown = HandleUpgrade(_player.DashCoolDown, up);
                    break;
            }

            _currentUpgrades.Clear();
        }
        float HandleUpgrade(float current, PlayerUpgrade up)
        {
            return up.Strat switch
            {
                UpgradeType.Addition => current + up.Value,
                UpgradeType.AddPercentage => current * (1 + up.Value / 100f),
                UpgradeType.SubPercentage => current * (1 - up.Value / 100f),
                UpgradeType.Subtraction => current - up.Value,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
