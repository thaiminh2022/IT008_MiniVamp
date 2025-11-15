using System.Numerics;

namespace IT008_Game.Core.Components
{
    /// <summary>
    /// A system to hold any character changeable value
    /// </summary>
    internal class ValueSystem<TType> where TType : INumber<TType>
    {
        TType _maxValue;
        TType _value;

        public ValueSystem(TType maxValue)
        {
            _maxValue = maxValue;
            _value = maxValue;
        }

        public TType GetValue()
        {
            return _value;
        }
        public TType GetMaxValue()
        {
            return _maxValue;
        }
        public TType GetValueNormalized()
        {
            if (_maxValue == TType.Zero) return TType.Zero;
            return _value / _maxValue;
        }

        public virtual void AddValue(TType amount)
        {
            _value += amount;
            if (_value > _maxValue) {
                _value = _maxValue;
            }
        }
        public virtual void ResetToFull()
        {
            _value = _maxValue;
        }

        public virtual void SetMaxValue(TType newMax, bool setValueToMax = false)
        {
            _maxValue = newMax;
            if (setValueToMax)
            {
                _value = _maxValue;
            }
        }

        public virtual void SubstractValue(TType amount) {
            _value -= amount;
            if (_value < TType.Zero)
            {
                _value = TType.Zero;
            }
        }
    }
}
