using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetSpawnPointAsTarget Action", menuName = "configs/StateMachine/Actions/SetSpawnPointAsTarget")]
    public class SetSpawnPointAsTargetAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.TargetPoint = context.BuyerSpawnPoints
                                            .SpawnPoints[Random.Range(0, context.BuyerSpawnPoints.SpawnPoints.Count - 1)];
            context.TP = context.TargetPoint.Target.position;
            context.SetLongDistanceToTarget();
        }
    }
}