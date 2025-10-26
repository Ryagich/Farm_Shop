using UnityEngine;

namespace Inventory.Item
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "configs/Inventory/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public ItemHolder HandPrefab { get; private set; } = null!;
        [field: SerializeField] public string ItemName { get; private set; } = "Item Name";
        [field: SerializeField] public float height { get; private set; } = .05f;
        [field: SerializeField, Min(1)] public int Price { get; private set; } = 1;
    }
}