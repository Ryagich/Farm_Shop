using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "GoToQueueTransition", menuName = "configs/StateMachine/Transitions/GoToQueue")]
    public class GoToQueueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            if (context.BuyPositions.All(p => p.Count is 0))
            {
                return false;
            }
            foreach (var position in context.BuyPositions)
            {
                if (position.Count >= position.Need)
                    continue;
                if (context.ShelvesController
                           .PositionsAtShelvesByTypes
                           .TryGetValue(position.Config, out var type))
                {
                    var shelves = type.Where(p => p.Key.CanGet()
                                          && p.Value.Any(any => any.IsFree 
                                                     || (context.UsedInfoAboutPositionAtShelfForBuyer is not null 
                                                     && context.UsedInfoAboutPositionAtShelfForBuyer == any)))
                                      .ToArray();
                    if (shelves.Length > 0)
                        return false;
                }
            }
            
            return true;
        }
    }
}