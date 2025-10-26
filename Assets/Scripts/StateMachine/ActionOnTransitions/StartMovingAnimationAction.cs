using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "StartMovingAnimation Action", menuName = "configs/StateMachine/Actions/StartMovingAnimation")]
    public class StartMovingAnimationAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, true);
        }
    }
}