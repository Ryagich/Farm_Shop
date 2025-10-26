using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
{
    [CreateAssetMenu(fileName = "GoAway Condition", menuName = "configs/StateMachine/Conditions/GoAway")]
    public class GoAwayCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            var result = true;
            foreach (var position in context.BuyPositions)
            {
                if (position.Count is not 0)
                {
                    result = false;
                    break;         
                }
                if (context.ShelvesController
                           .PositionsAtShelvesByTypes
                           .TryGetValue(position.Config, out var type))
                {
                    var shelves = type.Where(p => p.Key.CanGet()
                                               && p.Value.Any(any => any.IsFree 
                                                           || (context.UsedInfoAboutPositionAtShelfForBuyer is not null 
                                                            && context.UsedInfoAboutPositionAtShelfForBuyer == any)))
                                      .ToArray();
                    if (shelves.Length <= 0)
                       continue;
                    result = false;
                    break;
                }
            }
            if (result)
            {
                context.ClearInfoAboutShelf();
                context.SetLongDistanceToTarget();
                context.TargetPosition = context.SpawnPointsForBuyers[Random.Range(0, context.SpawnPointsForBuyers.Count - 1)].position;
            }
            return result;
        }
    }
}