using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.ActionOnTransitions
{
    [CreateAssetMenu(fileName = "ChoseShelf Action", menuName = "configs/StateMachine/Actions/ChoseShelf")]
    public class ChoseShelfAction : ActionOnTransitionBase
    {
        public override void DoAction(StateMachineContext context)
        {
            foreach (var position in context.BuyPositions)
            {
                if (position.Count < position.Need 
                 && context.ShelvesController
                                             .PositionsAtShelvesByTypes
                                             .TryGetValue(position.Config, out var type)
                   )
                {
                    var shelves = type.Where(shelf => shelf.Key.CanGet() 
                                                   && shelf.Value
                                                           .Any(any => any.IsFree.Value
                                                                     && any.BuildingInteractableFlag.IsInteractable))
                                      .ToArray();
                    if (shelves.Length <= 0)
                        continue;
                    
                    context.ClearInfoAboutShelf();
                    var shelf = shelves[Random.Range(0, shelves.Length - 1)];
                    var freePositions = shelf.Value
                                             .Where(p => p.IsFree.Value)
                                             .ToArray();
                    var positionForBuyer = freePositions[Random.Range(0, freePositions.Length - 1)];
                    positionForBuyer.IsFree.Value = false;
                    context.TargetPoint = positionForBuyer.TargetPoint;
                    context.TP = context.TargetPoint.Target.position;
                    context.UsedInfoAboutPositionAtShelfForBuyer = positionForBuyer;
                }
            }
        }
    }
}