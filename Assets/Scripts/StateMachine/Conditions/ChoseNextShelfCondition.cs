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
                && context.TargetBuyPosition.Config != null
             && context.UsedInfoAboutPositionAtShelfForBuyer.ShelfInventory.CanGet(context.TargetBuyPosition.Config))
            {
                var config = context.TargetBuyPosition.Config;
                var buyPos = context.BuyPositions.First(p => p.Config.Id.Equals(config.Id));
                if (context.Inventory.CanAdd(config)
                 && buyPos.Count.Value < buyPos.Need)
                {
                    return false;
                }
            }
            foreach (var position in context.BuyPositions)
            {
                if (position.Count.Value < position.Need
                 && context.ShelvesController.Shelves.Any(shelf => shelf.Key.CanGet(position.Config)))
                {
                    var shelves = context.ShelvesController.Shelves.Where(shelf => shelf.Key.HaveItem 
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