namespace IT008_Game.Game.GameObjects.PlayerCharacter
{
    enum UpgradeOption
    {
        AttackSpeed,
        BaseDamage,
        MaxHealth,
        MoveSpeed,
        DashTime,
    }
    enum UpgradeType
    {
        Addition,
        AddPercentage,
        SubPercentage,
        Subtraction,
    }

    internal record PlayerUpgrade
    {
        public readonly UpgradeOption Option;
        public readonly UpgradeType Strat;
        public readonly float Value;

        public PlayerUpgrade(UpgradeOption option, UpgradeType strat, float value)
        {
            Option = option;
            Strat = strat;
            Value = value;
        }

        public static PlayerUpgrade GetRandom()
        {
            var rng = new Random();
            var options = Enum.GetValues(typeof(UpgradeOption));
            var strats = Enum.GetValues(typeof(UpgradeType));

            var option = (UpgradeOption)options.GetValue(rng.Next(options.Length))!;
            var strat = (UpgradeType)options.GetValue(rng.Next(strats.Length))!;

            var value = strat switch
            {
                UpgradeType.Addition => rng.Next(2, 4),
                UpgradeType.Subtraction => rng.Next(5, 10),
                UpgradeType.AddPercentage => rng.Next(10, 30),
                UpgradeType.SubPercentage => rng.Next(36, 68),
                _ => throw new NotImplementedException(),
            };

            return new PlayerUpgrade(option, strat, value);
        }
    }
}
