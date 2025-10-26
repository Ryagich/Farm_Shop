using System.Diagnostics.CodeAnalysis;
using MessagePipe;
using Messages;
using UnityEngine;
using VContainer;

namespace Interactable
{
    public class InteractableFounder : MonoBehaviour
    {
        private InteractableConfig config;
        private IPublisher<InteractableMessage> interactablePublisher;
        private IPublisher<InteractableEndMessage> interactableEndPublisher;

        [Inject]
        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void Construct
            (
                InteractableConfig config,
                IPublisher<InteractableMessage> interactablePublisher,
                IPublisher<InteractableEndMessage> interactableEndPublisher
            )
        {
            this.config = config;
            this.interactablePublisher = interactablePublisher;
            this.interactableEndPublisher = interactableEndPublisher;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & config.InteractiveLayers) != 0)
            {
                var interactable = other.GetComponentInParent<Interactable>();
                if (interactable)
                    interactablePublisher.Publish(new InteractableMessage(interactable));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & config.InteractiveLayers) != 0)
            {
                var interactable = other.GetComponentInParent<Interactable>();
                if (interactable)
                    interactableEndPublisher.Publish(new InteractableEndMessage(interactable));
            }
        }
    }
}