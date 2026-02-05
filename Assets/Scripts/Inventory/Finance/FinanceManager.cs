using UniRx;
using YG;

namespace Inventory.Finance
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FinanceManager
    {
        // public ReactiveProperty<int> Value { get; private set; } = new(10000);
        public ReactiveProperty<int> Value { get; private set; } = new(YG2.saves.money);

        public bool TryChangeValue(int amount)
        {
            if (Value.Value + amount < 0)
                return false;
            Value.Value += amount;
            YG2.saves.money = Value.Value;
            YG2.SaveProgress();
            return true;
        }

        public bool Check(int value) => Value.Value >= value;
    }
}