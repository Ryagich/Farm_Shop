using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "Empty Behaviour", menuName = "configs/StateMachine/Behaviours/Empty")]
    public class EmptyBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context) { }

        public override void Logic(StateMachineContext context) { }

        public override void Exit(StateMachineContext context) { }
    }
}