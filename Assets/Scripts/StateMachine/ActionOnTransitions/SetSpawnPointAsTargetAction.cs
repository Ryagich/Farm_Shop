using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "SetSpawnPointAsTarget Action", menuName = "configs/StateMachine/Actions/SetSpawnPointAsTarget")]
    public class SetSpawnPointAsTargetAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.TargetPosition = context.SpawnPointsForBuyers[Random.Range(0, context.SpawnPointsForBuyers.Count - 1)].position;
            context.SetLongDistanceToTarget();
        }
    }
}