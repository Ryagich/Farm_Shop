using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "GoToTargetPoint", menuName = "configs/StateMachine/Behaviours/GoToTargetPoint")]
    public class BehaviourGoToTargetPoint : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, true);
        }
        
        public override void Logic(StateMachineContext context)
        {
            context.NavMeshAgent.SetDestination(context.TargetPosition);
        }

        public override void Exit(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, false);
        }
    }
}