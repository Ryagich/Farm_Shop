using System.Collections.Generic;
using Container;
using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Interactable
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerInteractableLogic : IStartable, IFixedTickable
    {
        private readonly PlayerLifetimeScope scope;
        private readonly InteractableConfig config;

        private readonly CompositeDisposable disposables = new();
        private float t;
        private readonly List<Interactable> Interactables = new();
            
        public PlayerInteractableLogic
            (
                ISubscriber<InteractableMessage> interactableSubscriber,
                ISubscriber<InteractableEndMessage> interactableEndSubscriber,
                PlayerLifetimeScope scope,
                InteractableConfig config
            )
        {
            this.scope = scope;
            this.config = config;
            interactableSubscriber.Subscribe(Add).AddTo(disposables);
            interactableEndSubscriber.Subscribe(Remove).AddTo(disposables);
        }

        public void Start() { }

        private void Add(InteractableMessage msg)
        {
            Interactables.Add(msg.Interactable);
        }

        private void Remove(InteractableEndMessage msg)
        {
            Interactables.Remove(msg.Interactable);
            msg.Interactable.EndInteract(scope);
            if (Interactables.Count is 0)
                t = .0f;
        }
        
        public void FixedTick()
        {
            if (t > config.TimeBetweenInteractions)
            {
                foreach (var interactable in Interactables)
                    interactable.Interact(scope);
                t = .0f;
            }
            if (Interactables.Count > 0)
                t += Time.fixedDeltaTime;
        }
    }
}