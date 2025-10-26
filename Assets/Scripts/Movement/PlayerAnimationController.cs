using MessagePipe;
using Messages;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Movement
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerAnimationController : IStartable
    {
        private readonly PlayerMovementConfig playerMovementConfig;
        private readonly Animator animator;
        private readonly CompositeDisposable disposables = new();

        public PlayerAnimationController
            (
                PlayerMovementConfig playerMovementConfig,
                ISubscriber<PlayerMoveMessage> subscriber,
                Animator animator
            )
        {
            this.playerMovementConfig = playerMovementConfig;
            this.animator = animator;

            subscriber.Subscribe(OnVelocityChanged).AddTo(disposables);  
        }
        
        private void OnVelocityChanged(PlayerMoveMessage msg)
        {
            animator.SetBool(playerMovementConfig.MovingName, msg.Direction is not {x: 0, y: 0});
        }

        public void Start() { }
    }
}