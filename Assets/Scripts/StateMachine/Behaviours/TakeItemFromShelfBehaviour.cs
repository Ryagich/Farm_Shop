using System.Linq;
using StateMachine.Graph.Model;
using UnityEngine;

namespace StateMachine.Behaviours
{
    [CreateAssetMenu(fileName = "TakeItemFromShelf Behaviour", menuName = "configs/StateMachine/Behaviours/TakeItemFromShelf")]
    public class TakeItemFromShelfBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            SetDefaultParameters(context);
        }
        
        public override void Logic(StateMachineContext context)
        {
            var shelfInventory = context.UsedInfoAboutPositionAtShelfForBuyer.ShelfInventory;

            if (context.TimeBetweenIterations > context.T
             || !shelfInventory.CanGet())
            {
                context.T += context.DeltaTime;
                return;
            }

            var config = shelfInventory.GetConfig();
            var buyPos = context.BuyPositions.First(p => p.Config == config);

            if (context.Inventory.CanAdd(config)
             && buyPos.Count < buyPos.Need)
            {
                var itemHolder = shelfInventory.Get();
                context.Inventory.Add(config, itemHolder.transform.localToWorldMatrix);
                Destroy(itemHolder.gameObject);
                SetDefaultParameters(context);
                buyPos.Count++;
                return;
            }
            context.T += context.DeltaTime;
        }

        public override void Exit(StateMachineContext context)
        {
            SetDefaultParameters(context);
        }

        private void SetDefaultParameters(StateMachineContext context)
        {
            context.TimeBetweenIterations = Random.Range(context.BuyerSettings.SlowlyTimeBetweenInteraction.x,
                                                         context.BuyerSettings.SlowlyTimeBetweenInteraction.y);
            context.T = .0f;
        }
    }
}