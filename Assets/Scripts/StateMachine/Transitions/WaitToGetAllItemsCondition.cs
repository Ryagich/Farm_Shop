using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "WaitToGetAllItems Condition", menuName = "configs/StateMachine/Conditions/WaitToGetAllItems")]
    public class WaitToGetAllItemsCondition : BaseCondition
    {
            public override bool IsCondition(StateMachineContext context)
                => context.Inventory.Items.Count <= 0;
    }
}