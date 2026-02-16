using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;
using Storage;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShoppingListGenerator
    {
        private readonly BuyerSettings buyerSettings;
        private readonly ItemsStorage itemsStorage;

        public ShoppingListGenerator
            (
                BuyerSettings buyerSettings,
                ItemsStorage itemsStorage
            )
        {
            this.buyerSettings = buyerSettings;
            this.itemsStorage = itemsStorage;
        }
        
        public IEnumerable<BuyPosition> GetPositions()
        {
            var chance = 1.0f;
            var items = itemsStorage.Items.ToList(); 
            while (chance > 0 && items.Count > 0)
            {
                if (chance >= Random.Range(.0f, 1.0f))
                {
                    var i = Random.Range(0, items.Count - 1);
                    var item = items[i];
                    items.Remove(item);
                    yield return new BuyPosition(item.ItemConfig, Random.Range(buyerSettings.PurchaseRange.x, 
                                                                    buyerSettings.PurchaseRange.y));
                }
                chance -= buyerSettings.ChanceDecrease;
            }
        }
    }
}