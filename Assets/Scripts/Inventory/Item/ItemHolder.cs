using UnityEngine;

namespace Inventory.Item
{
    public class ItemHolder : MonoBehaviour
    {
        [field: SerializeField] public ItemConfig Config { get; private set; } = null!;
    }
}