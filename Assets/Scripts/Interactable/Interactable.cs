using System;
using UnityEngine;
using VContainer.Unity;

namespace Interactable
{
    public class Interactable : MonoBehaviour
    {
        public event Action<LifetimeScope> Interacted;
        public event Action<LifetimeScope> EndInteracted;

        public void Interact(LifetimeScope scope)
        {
            Interacted?.Invoke(scope);
        }
        
        public void EndInteract(LifetimeScope scope)
        {
            EndInteracted?.Invoke(scope);
        }
    }
}