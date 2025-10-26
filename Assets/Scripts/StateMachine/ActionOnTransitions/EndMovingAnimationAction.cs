using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions

{
    [CreateAssetMenu(fileName = "EndMovingAnimation Action", menuName = "configs/StateMachine/Actions/EndMovingAnimation")]
    public class EndMovingAnimationAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            context.Animator.SetBool(context.BuyerSettings.MovingName, false);
        }
    }
}