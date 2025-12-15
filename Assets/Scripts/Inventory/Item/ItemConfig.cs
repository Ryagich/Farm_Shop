using UnityEngine;
using UnityEngine.Localization;

namespace Inventory.Item
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "configs/Inventory/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [field: SerializeField] public ItemHolder HandPrefab { get; private set; } = null!;
        [field: SerializeField] public LocalizedString Name { get; private set; }
        [field: SerializeField] public float height { get; private set; } = .05f;
        [field: SerializeField, Min(1)] public int Price { get; private set; } = 1;
    }
}