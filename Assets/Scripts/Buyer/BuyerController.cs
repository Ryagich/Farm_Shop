using System.Collections.Generic;
using System.Linq;
using Checkout;
using Inventory;
using MessagePipe;
using Messages;
using Shelf;
using StateMachine;
using StateMachine.Graph.Model;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;

namespace Buyer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BuyerController : IFixedTickable
    {
        private readonly BuyerLifetimeScope buyerLifetimeScope;
        public State CurrentState { get; private set; }

        public StateMachineContext context;
        
        public BuyerController
            (
                BuyerSettings buyerSettings,
                ShoppingListGenerator shoppingListGenerator,
                NavMeshAgent navMeshAgent,
                ShelvesController shelvesController,
                BuyerLifetimeScope buyerLifetimeScope,
                CheckoutsController checkoutsController,
                NavMeshSurface navMeshSurface,
                Animator animator,
                IInventory inventory,
                [Key("ShoppingEnter")] Transform ShoppingEnter,
                [Key("Hand")] Transform hand,
                [Key("SpawnPointsForBuyers")] List<Transform> spawnPointsForBuyers,
                IPublisher<BuyerIsOverMessage> buyerIsOverPublisher
            )
        {
            this.buyerLifetimeScope = buyerLifetimeScope;
            context = new StateMachineContext();
            context.BuyerSettings = buyerSettings;
            context.BuyPositions = shoppingListGenerator.GetPositions().ToList();
            context.NavMeshAgent = navMeshAgent;
            context.ShelvesController = shelvesController;
            context.Inventory = inventory;
            context.ShoppingEnter = ShoppingEnter;
            context.SpawnPointsForBuyers = spawnPointsForBuyers;
            context.BuyerLifetimeScope = buyerLifetimeScope;
            context.CheckoutsController = checkoutsController;
            context.BuyerIsOverPublisher = buyerIsOverPublisher;
            context.Hand = hand;
            context.NavMeshSurface = navMeshSurface;
            context.Animator = animator;
            
            SetState(buyerSettings.StateMachineGraph.GetEntryState());
        }

        public void FixedTick()
        {
            if (CurrentState == null)
            {
                return;
            }
            context.DeltaTime = Time.fixedDeltaTime;

            foreach (var behaviour in CurrentState.Behaviours)
            {
                behaviour.Logic(context);
            }
            
            foreach (var transition in CurrentState.Transitions)
            {
                if (transition.CanTransition(context))
                {
                    foreach (var action in transition.ActionOnTransitions)
                    {
                        action.DoAction(context);
                    }
                    SetState(transition.TargetState);
                    break;
                }
            }
        }
        
        private void SetState(State state)
        {
            if (CurrentState != null)
            {
                foreach (var behaviour in CurrentState.Behaviours)
                {
                    behaviour.Exit(context);
                }
            }
            
            
            CurrentState = state;
            foreach (var behaviour in CurrentState.Behaviours)
            {
                behaviour.Enter(context);
            }
            
            buyerLifetimeScope.CurrentState = state;
        }
    }
}