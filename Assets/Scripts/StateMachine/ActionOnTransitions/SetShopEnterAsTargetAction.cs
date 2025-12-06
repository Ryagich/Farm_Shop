using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetShopEnterAsTarget Action", menuName = "configs/StateMachine/Actions/SetShopEnterAsTarget")]
    public class SetShopEnterAsTargetAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            var c = context.DoorsController.DoorPoints.Count;
            context.TargetPoint = context.DoorsController.DoorPoints[Random.Range(0, c)];
            context.TP = context.TargetPoint.Target.position;
            context.SetLongDistanceToTarget();
        }
    }
}