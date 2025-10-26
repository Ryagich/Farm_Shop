using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "InventoryConfig", menuName = "configs/Inventory/InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxCount { get; private set; }
    }
}