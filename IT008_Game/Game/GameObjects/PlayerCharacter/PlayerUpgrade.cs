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
        private static readonly Random _rng = new();

        public readonly UpgradeOption Option;
        public readonly UpgradeType Strat;
        /// <summary>
        /// For *Percentage types* this is in percent (e.g. 10 = +10%).
        /// For *Addition/Subtraction* this is the raw value to add or subtract.
        /// </summary>
        public readonly float Value;

        public PlayerUpgrade(UpgradeOption option, UpgradeType strat, float value)
        {
            Option = option;
            Strat = strat;
            Value = value;
        }

        public static PlayerUpgrade GetRandom()
        {
            var options = (UpgradeOption[])Enum.GetValues(typeof(UpgradeOption));
            var strats = (UpgradeType[])Enum.GetValues(typeof(UpgradeType));

            // Pick which stat to upgrade
            var option = options[_rng.Next(options.Length)];

            // If you want mostly positive upgrades, bias here instead of pure random:
            // e.g. 0-69 = AddPercentage, 70-89 = Addition, 90-94 = SubPercentage, 95-99 = Subtraction
            var roll = _rng.Next(100);
            UpgradeType strat;
            if (roll < 70) strat = UpgradeType.AddPercentage;
            else if (roll < 90) strat = UpgradeType.Addition;
            else if (roll < 95) strat = UpgradeType.SubPercentage;
            else strat = UpgradeType.Subtraction;

            float value = strat switch
            {
                UpgradeType.AddPercentage => GetAddPercent(option),
                UpgradeType.Addition => GetAddFlat(option),
                UpgradeType.SubPercentage => GetSubPercent(option),
                UpgradeType.Subtraction => GetSubFlat(option),
                _ => 0f
            };

            return new PlayerUpgrade(option, strat, value);
        }

        // ----------------- helpers -----------------

        private static float GetAddPercent(UpgradeOption option) => option switch
        {
            // As per balance notes:
            // AS / DMG ~10–20%, HP ~15–25%, Move ~5–12%, DashTime ~5–10%
            UpgradeOption.AttackSpeed => _rng.Next(10, 21), // 10–20%
            UpgradeOption.BaseDamage => _rng.Next(10, 21), // 10–20%
            UpgradeOption.MaxHealth => _rng.Next(15, 26), // 15–25%
            UpgradeOption.MoveSpeed => _rng.Next(5, 13),  // 5–12%
            UpgradeOption.DashTime => _rng.Next(5, 11),  // 5–10%
            _ => 10
        };

        private static float GetAddFlat(UpgradeOption option) => option switch
        {
            // These assume starting stats around:
            // BaseDamage 10, MaxHealth 100, AttackSpeed 2, MoveSpeed 4, DashTime 0.25
            UpgradeOption.AttackSpeed => Lerp(0.05f, 0.30f), // +0.05–0.30 attacks/sec
            UpgradeOption.BaseDamage => _rng.Next(2, 6),    // +2–5 damage
            UpgradeOption.MaxHealth => _rng.Next(10, 26),  // +10–25 HP
            UpgradeOption.MoveSpeed => Lerp(0.20f, 0.70f), // +0.2–0.7 units/sec
            UpgradeOption.DashTime => Lerp(0.01f, 0.04f), // +0.01–0.04s i-frames
            _ => 1f
        };

        private static float GetSubPercent(UpgradeOption option) => option switch
        {
            // Penalties – keep them modest so they feel like tradeoffs, not trolling.
            UpgradeOption.AttackSpeed => _rng.Next(5, 11),   // -5–10%
            UpgradeOption.BaseDamage => _rng.Next(5, 16),   // -5–15%
            UpgradeOption.MaxHealth => _rng.Next(10, 21),  // -10–20%
            UpgradeOption.MoveSpeed => _rng.Next(5, 13),   // -5–12%
            UpgradeOption.DashTime => _rng.Next(3, 9),    // -3–8% dash duration
            _ => 5
        };

        private static float GetSubFlat(UpgradeOption option) => option switch
        {
            UpgradeOption.AttackSpeed => Lerp(0.05f, 0.20f), // -0.05–0.20 attacks/sec
            UpgradeOption.BaseDamage => _rng.Next(1, 4),    // -1–3 damage
            UpgradeOption.MaxHealth => _rng.Next(5, 16),   // -5–15 HP
            UpgradeOption.MoveSpeed => Lerp(0.10f, 0.40f), // -0.1–0.4 units/sec
            UpgradeOption.DashTime => Lerp(0.005f, 0.02f),// -0.005–0.02s
            _ => 1f
        };

        private static float Lerp(float min, float max)
        {
            return min + (float)_rng.NextDouble() * (max - min);
        }
    }
}
