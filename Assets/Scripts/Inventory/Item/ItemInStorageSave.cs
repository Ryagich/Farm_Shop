using System;

namespace Inventory.Item
{
    [Serializable]
    public class ItemInStorageSave
    {
        public string Id;
        public int Count;
        
        public ItemInStorageSave(string id, int count)
        {
            Id = id;
            Count = count;
        }
    }
}