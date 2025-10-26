using StateMachine.Graph;
using UnityEngine;

namespace Buyer
{
    [CreateAssetMenu(fileName = "BuyerSettings", menuName = "configs/Buyer/BuyerSettings")]
    public class BuyerSettings : ScriptableObject
    {
        [field: SerializeField] public StateMachineGraph StateMachineGraph { get; private set; } = null!;
        [field: SerializeField, Min(.0f)] public float MoveSpeed { get; private set; } = 3.0f;
        [field: SerializeField, Range(.0f, 1.0f)] public float ChanceDecrease { get; private set; } = .2f;
        [field: SerializeField] public Vector2Int PurchaseRange { get; private set; } = new Vector2Int(1, 4);
        [field: SerializeField] public Vector2 LongRange { get; private set; } = new (1.0f, 2.0f);
        [field: SerializeField] public Vector2 ShortRange { get; private set; } = new (.05f, .5f);
        [field: SerializeField] public Vector2 SlowlyTimeBetweenInteraction { get; private set; } = new (0.7f, 3.0f);
        [field: SerializeField] public Vector2 FastTimeBetweenInteraction { get; private set; } = new (0.1f, 0.2f);
        [field: SerializeField] public float QueueDistance { get; private set; } = 1.5f;
        [field: SerializeField] public string MovingName { get; private set; } = "IsMoving";
        [field: SerializeField] public int CriticalCountOfQueue { get; private set; } = 4;
        [field: SerializeField] public float ChanceToLeaveForExtraBuyer { get; private set; } = .1f;
        [field: SerializeField] public Color OutOfStockColor { get; private set; } = Color.gray;
        [field: SerializeField] public Color NotForSaleColor { get; private set; } = Color.red;
    }
}