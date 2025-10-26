using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerParticleController : IStartable
    {
        private ParticleSystem.EmissionModule emission;

        private readonly CompositeDisposable disposables = new();

        private PlayerParticleController
            (
                ParticleSystem particleSystem,
                ISubscriber<PlayerMoveMessage> subscriber
            )
        {
            emission = particleSystem.emission;
            emission.enabled = false;
            
            subscriber.Subscribe(EnableParticle).AddTo(disposables);  
        }
        
        private void EnableParticle(PlayerMoveMessage msg)
            => emission.enabled = msg.Direction is not { x: 0, y: 0 };

        public void Start() { }
    }
}