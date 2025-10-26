using UniRx;

namespace Inventory.Finance
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FinanceManager
    {
        public ReactiveProperty<int> Value { get; private set; } = new();
        
        public bool TryChangeValue(int amount)
        {
            if (Value.Value + amount < 0)
                return false;
            Value.Value += amount;
            return true;
        }

        public bool Check(int value) => Value.Value >= value;
    }
}