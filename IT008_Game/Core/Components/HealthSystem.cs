namespace IT008_Game.Core.Components
{
    /// <summary>
    /// A system responsible for anything related to health
    /// </summary>
    internal class HealthSystem : ValueSystem<float>
    {
        public event EventHandler? OnDead;
        public event EventHandler? OnHealthChange;

        bool _isDead;
        public bool IsDead => _isDead;

        public HealthSystem(float maxValue) : base(maxValue)
        {
            _isDead = false;
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
            if (_isDead)
                return;

            base.SubstractValue(amount);
            OnHealthChange?.Invoke(this, EventArgs.Empty);

            if (GetValue() <= 0f)
            {
                _isDead = true;
                OnDead?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
