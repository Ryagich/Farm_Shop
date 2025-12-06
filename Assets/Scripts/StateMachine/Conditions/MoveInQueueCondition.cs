using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "MoveInQueue Condition", menuName = "configs/StateMachine/Conditions/MoveInQueue")]
    public class MoveInQueueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            var queue = context.CheckoutController.ByersQueue;
            if (Vector3.Distance(context.NavMeshAgent.transform.position,
                                 context.CheckoutController.ByersQueue.GetBuyerPosition(context.QueueIndex).Item1.Target.position) 
                                    > context.DistanceToTarget)
            {
                var pos = queue.GetBuyerPosition(context.QueueIndex);
                context.TargetPoint = pos.Item1;
                pos.Item2 = false;
                context.TP = context.TargetPoint.Target.position;
                return true;
            }
            return false;
        }
    }
}