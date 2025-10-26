using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
  	[CreateAssetMenu(fileName = "ClearInfoAboutShelf Action", menuName = "configs/StateMachine/Actions/ClearInfoAboutShelf")]
  	public class ClearInfoAboutShelfAction : ActionOnTransitionBase
    {
         public override void DoAction(StateMachineContext context)
        {
            context.TargetPosition = context.SpawnPointsForBuyers[Random.Range(0, context.SpawnPointsForBuyers.Count - 1)].position;
            context.SetLongDistanceToTarget();
        }
    }
}