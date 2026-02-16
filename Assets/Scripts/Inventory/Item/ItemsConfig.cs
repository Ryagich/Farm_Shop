using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Item
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "configs/Inventory/ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 7f;
        [field: SerializeField] public float Gamma { get; private set; } = 1f;
    }
}