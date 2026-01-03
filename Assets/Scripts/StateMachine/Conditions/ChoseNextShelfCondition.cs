using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Conditions
{
    [CreateAssetMenu(fileName = "ChoseNextShelf Condition", menuName = "configs/StateMachine/Conditions/ChoseNextShelf")]
    public class ChoseNextShelfCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context)
        {
            if (context.UsedInfoAboutPositionAtShelfForBuyer != null
             && context.UsedInfoAboutPositionAtShelfForBuyer.ShelfInventory.CanGet())
            {
                var config = context.UsedInfoAboutPositionAtShelfForBuyer.ShelfInventory.GetConfig();
                var buyPos = context.BuyPositions.First(p => p.Config.ID.Equals(config.ID));
                if (context.Inventory.CanAdd(config)
                 && buyPos.Count.Value < buyPos.Need)
                {
                    // Debug.Log($"Используем еще прошлую полку");
                    return false;
                }
            }
            foreach (var position in context.BuyPositions)
            {
                if (position.Count.Value < position.Need
                 && context.ShelvesController
                                             .PositionsAtShelvesByTypes
                                             .TryGetValue(position.Config.ID, out var type)
                   )
                {
                    var shelves = type.Where(shelf => shelf.Key.CanGet() 
                                                   && shelf.Value.Any(any => any.IsFree.Value
                                                                      && any.BuildingInteractableFlag.IsInteractable))
                                      .ToArray();
                    if (shelves.Length <= 0)
                        continue;
                    
                    return true;
                }
            }
            return false;
        }
    }
}