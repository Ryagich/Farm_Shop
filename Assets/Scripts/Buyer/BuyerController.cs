using System.Collections.Generic;
using System.Linq;
using Checkout;
using Doors;
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
                SurfaceController surfaceController,
                Animator animator,
                IInventory inventory,
                DoorsController doorsController,
                [Key("Hand")] Transform hand,
                BuyerSpawnPoints buyerSpawnPoints,
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
            context.DoorsController = doorsController;
            context.BuyerSpawnPoints = buyerSpawnPoints;
            context.BuyerLifetimeScope = buyerLifetimeScope;
            context.CheckoutsController = checkoutsController;
            context.BuyerIsOverPublisher = buyerIsOverPublisher;
            context.Hand = hand;
            context.SurfaceController = surfaceController;
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
            buyerLifetimeScope.TP = context.TP;
            buyerLifetimeScope.DistanceToTarget = context.DistanceToTarget;
            buyerLifetimeScope.QueueIndex = context.QueueIndex;
            if (context.TargetPoint != null)
                buyerLifetimeScope.targetPoint = context.TargetPoint.Target;
            else
                buyerLifetimeScope.targetPoint = null;
        }
    }
}