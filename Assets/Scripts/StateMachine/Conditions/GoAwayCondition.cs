using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "GoAway Condition", menuName = "configs/StateMachine/Conditions/GoAway")]
    public class GoAwayCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            var result = true;
            foreach (var position in context.BuyPositions)
            {
                if (position.Count.Value is not 0)
                {
                    result = false;
                    break;         
                }
                if (context.ShelvesController
                           .PositionsAtShelvesByTypes
                           .TryGetValue(position.Config.ID, out var type))
                {
                    var shelves = type.Where(p => p.Key.CanGet()
                                               && p.Value.Any(any => (any.IsFree.Value && any.BuildingInteractableFlag.IsInteractable)
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
                context.TargetPoint = context.BuyerSpawnPoints
                                             .SpawnPoints
                                                  [Random.Range(0, context.BuyerSpawnPoints.SpawnPoints.Count - 1)];
                context.TP = context.TargetPoint.Target.position;
            }
            return result;
        }
    }
}