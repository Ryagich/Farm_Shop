using System;
using Inventory.Item;
using UniRx;

namespace Buyer
{
    [Serializable]
    public class BuyPosition
    {
        public ItemConfig Config { get; private set; }
        public int Need { get; private set; }
        public ReactiveProperty<int> Count { get; set; } = new();

        public BuyPosition(ItemConfig config, int need)
        {
            Config = config;
            Need = need;
        }
    }
}