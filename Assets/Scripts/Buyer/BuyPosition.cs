using System;
using Inventory.Item;

namespace Buyer
{
    [Serializable]
    public class BuyPosition
    {
        public ItemConfig Config { get; private set; }
        public int Need { get; private set; }
        public int Count { get; set; }

        public BuyPosition(ItemConfig config, int need)
        {
            Config = config;
            Need = need;
        }
    }
}