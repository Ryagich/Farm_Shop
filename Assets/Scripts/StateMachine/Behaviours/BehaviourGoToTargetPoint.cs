using StateMachine.Graph.Model;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "GoToTargetPoint", menuName = "configs/StateMachine/Behaviours/GoToTargetPoint")]
    public class BehaviourGoToTargetPoint : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, true);
            context.NavMeshAgent.SetDestination(context.TP);
            context.T = .0f;
        }
        
        public override void Logic(StateMachineContext context)
        {
            if (context.NavMeshAgent.pathPending)
                return;

            if (context.NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                TrySetDestination(context);
                return;
            }
            if (context.NavMeshAgent.velocity.sqrMagnitude < 0.01f) 
            {
                context.T += Time.deltaTime;
                if (context.T > 1.0f)
                {
                    TrySetDestination(context);
                    context.T = 0f;
                }
            }
            else
            {
                context.T = 0f;
            }
        }

        public override void Exit(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, false);
        }
        
        private void TrySetDestination(StateMachineContext context)
        {
            if (context.TargetPoint == null || context.TargetPoint.Target == null)
                return;

            var tp = context.TargetPoint.Target.position;

            if (NavMesh.SamplePosition(tp, out var hit, 2f, NavMesh.AllAreas))
            {
                context.NavMeshAgent.SetDestination(hit.position);
            }
        }
    }
}