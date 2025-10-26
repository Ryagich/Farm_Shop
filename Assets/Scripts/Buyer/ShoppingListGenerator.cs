using System.Collections.Generic;
using Inventory.Item;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShoppingListGenerator
    {
        private readonly ItemsConfig itemsConfig;
        private readonly BuyerSettings buyerSettings;
        
        public ShoppingListGenerator(ItemsConfig itemsConfig, BuyerSettings buyerSettings)
        {
            this.itemsConfig = itemsConfig;
            this.buyerSettings = buyerSettings;
        }
        
        public IEnumerable<BuyPosition> GetPositions()
        {
            var chance = 1.0f;
            var items = itemsConfig.Items.ToList(); 
            while (chance > 0 && items.Count > 0)
            {
                if (chance >= Random.Range(.0f, 1.0f))
                {
                    var i = Random.Range(0, items.Count - 1);
                    var item = items[i];
                    items.Remove(item);
                    yield return new BuyPosition(item, Random.Range(buyerSettings.PurchaseRange.x, 
                                                                    buyerSettings.PurchaseRange.y));
                }
                chance -= buyerSettings.ChanceDecrease;
            }
        }
    }
}