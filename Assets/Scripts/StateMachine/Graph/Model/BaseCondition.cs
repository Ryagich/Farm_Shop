using UnityEngine;

namespace StateMachine.Graph.Model
{
    public abstract class BaseCondition : ScriptableObject
    {
        public virtual bool IsCondition(StateMachineContext context)
        {
            return false;
        }
    }
}