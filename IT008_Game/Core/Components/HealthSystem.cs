namespace IT008_Game.Core.Components
{
    /// <summary>
    /// A system responsible for anything related to health
    /// </summary>
    internal class HealthSystem : ValueSystem<float>
    {
        public event EventHandler? OnDead;
        public event EventHandler? OnHealthChange;

        public bool IsDead => _value <= 0;

        public HealthSystem(float maxValue) : base(maxValue)
        {
        }

        public override void AddValue(float amount)
        {
            base.AddValue(amount);
            OnHealthChange?.Invoke(this, EventArgs.Empty);
        }

        public override void ResetToFull()
        {
            base.ResetToFull();
            OnHealthChange?.Invoke(this, EventArgs.Empty);
        }
        public override void SubstractValue(float amount)
        {
            if (IsDead)
                return;

            base.SubstractValue(amount);
            OnHealthChange?.Invoke(this, EventArgs.Empty);

            if (GetValue() <= 0f)
            {
                OnDead?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
