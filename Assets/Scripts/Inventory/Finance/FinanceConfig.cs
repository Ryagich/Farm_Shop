using UnityEngine;

namespace Inventory.Finance
{
    [CreateAssetMenu(fileName = "FinanceConfig", menuName = "configs/Inventory/FinanceConfig")]
    public class FinanceConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxPurchaseTime { get; private set; } = 5.0f;
        [field: SerializeField] public GameObject MoneyPrefab { get; private set; } = null!;
    }
}