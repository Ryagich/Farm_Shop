using System.Collections.Generic;
using System.Linq;
using Inventory.Item;
using Inventory.ObjectInventory;

namespace Products
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MaterialComposer
    {
        private readonly ProductConfig productConfig;
        private readonly MaterialInventoriesController materialInventoriesController;

        public MaterialComposer
            (
                ProductConfig productConfig,
                MaterialInventoriesController materialInventoriesController
            )
        {
            this.productConfig = productConfig;
            this.materialInventoriesController = materialInventoriesController;
        }
        
        public IEnumerable<ItemHolder> GetMaterialsForProduct()
        {
            foreach (var material in productConfig.Materials)
            {
                var inventory =
                    materialInventoriesController.inventories.First(i => i.GetConfig() == material.ItemConfig);
                for (var i = 0; i < material.CountForProduct; i++)
                {
                   yield return inventory.Get();
                }
            }
        }

        public bool CheckCanCreateProduct()
        {
            foreach (var material in productConfig.Materials)
            {
                var inventory = materialInventoriesController.inventories.First(i => i.GetConfig() == material.ItemConfig);
                if (inventory.Items.Count < material.CountForProduct)
                    return false;
            }
            return true;
        }
    }
}