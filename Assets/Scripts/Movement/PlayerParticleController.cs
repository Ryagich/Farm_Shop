using MessagePipe;
using Messages;
using UnityEngine;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerParticleController
    {
        private ParticleSystem.EmissionModule emission;

        private bool canPlay = true;
        
        private PlayerParticleController
            (
                ParticleSystem particleSystem,
                ISubscriber<PlayerMoveMessage> subscriber
            )
        {
            emission = particleSystem.emission;
            emission.enabled = false;
            
            subscriber.Subscribe(OnVelocityChanged);  
        }
        public void ChangeState(bool newState, Vector2 direction)
        {
            canPlay = newState;
            Update(direction);
        }
        
        private void OnVelocityChanged(PlayerMoveMessage msg)
        {
            Update(msg.Direction);
        }

        private void Update(Vector2 direction)
        {
            emission.enabled = canPlay && direction is not { x: 0, y: 0 };
        }
    }
}