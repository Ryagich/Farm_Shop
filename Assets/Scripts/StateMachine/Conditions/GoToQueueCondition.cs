using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "GoToQueue Condition", menuName = "configs/StateMachine/Conditions/GoToQueue")]
    public class GoToQueueCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            if (context.BuyPositions.All(p => p.Count.Value is 0))
            {
                return false;
            }
            foreach (var position in context.BuyPositions)
            {
                if (position.Count.Value >= position.Need)
                    continue;
                if (context.ShelvesController
                           .PositionsAtShelvesByTypes
                           .TryGetValue(position.Config, out var type))
                {
                    var shelves = type.Where(p => p.Key.CanGet()
                                          && p.Value.Any(any => any.IsFree.Value 
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