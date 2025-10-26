using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "MoveInQueue Condition", menuName = "configs/StateMachine/Conditions/MoveInQueue")]
    public class MoveInQueueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            var queue = context.CheckoutController.ByersQueue;
            if (Vector3.Distance(context.NavMeshAgent.transform.position,
                                 context.CheckoutController.ByersQueue.GetBuyerPosition(context.QueueIndex)) > context.DistanceToTarget)
            {
                context.TargetPosition = queue.GetBuyerPosition(context.QueueIndex);
                return true;
            }
            return false;
        }
    }
}