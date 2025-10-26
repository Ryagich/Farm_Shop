using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Transitions
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
                var buyPos = context.BuyPositions.First(p => p.Config == config);
                if (context.Inventory.CanAdd(config)
                 && buyPos.Count < buyPos.Need)
                {
                    Debug.Log($"Используем еще прошлую полку");
                    return false;
                }
            }
            foreach (var position in context.BuyPositions)
            {
                if (position.Count < position.Need
                 && context.ShelvesController
                                             .PositionsAtShelvesByTypes
                                             .TryGetValue(position.Config, out var type)
                   )
                {
                    var shelves = type.Where(shelf => shelf.Key.CanGet() 
                                                   && shelf.Value.Any(any => any.IsFree))
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